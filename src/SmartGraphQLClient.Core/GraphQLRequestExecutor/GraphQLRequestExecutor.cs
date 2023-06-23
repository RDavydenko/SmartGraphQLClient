using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Services.Abstractions;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Errors;
using SmartGraphQLClient.Exceptions;
using System;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartGraphQLClient.Core.GraphQLRequestExecutor
{
    internal class GraphQLRequestExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly object? _authorizationService;
        private readonly HttpClient _httpClient;
        private readonly GraphQLQueryBuilder.GraphQLQueryBuilder _queryBuilder;
        private readonly JsonSerializerOptions _jsonSerializerOptions =
            new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() },
                PropertyNameCaseInsensitive = true,
            };

        private GraphQLRequestExecutor(
            IServiceProvider serviceProvider,
            HttpClient httpClient,
            GraphQLQueryBuilder.GraphQLQueryBuilder queryBuilder,
            Type graphQLClientType)
        {
            _serviceProvider = serviceProvider;
            _authorizationService = _serviceProvider.GetService(typeof(IGraphQLAuthorizationService<>).MakeGenericType(graphQLClientType));
            _httpClient = httpClient;
            _queryBuilder = queryBuilder;
        }

        public static GraphQLRequestExecutor New(
            IServiceProvider serviceProvider,
            HttpClient httpClient,
            GraphQLQueryBuilder.GraphQLQueryBuilder queryBuilder,
            Type graphQLClientType)
        {
            return new(serviceProvider, httpClient, queryBuilder, graphQLClientType);
        }

        public Task<T[]> ExecuteToArrayAsync<T>(CancellationToken token)
        {
            _queryBuilder.ConfigureRequestOptions(x => x.IsArray = true);
            var query = _queryBuilder.Build(out var config);
            return ExecuteToArrayAsync<T>(query, config, token);
        }

        public Task<CollectionSegment<T>> ExecuteToPageAsync<T>(CancellationToken token)
        {
            _queryBuilder.ConfigureRequestOptions(x => x.IsCollectionPage = true);
            var query = _queryBuilder.Build(out var config);
            return ExecuteToPageAsync<T>(query, config, token);
        }

        public Task<T?> ExecuteFirstOrDefaultAsync<T>(CancellationToken token)
        {
            _queryBuilder.ConfigureRequestOptions(x => x.IsFirstOrDefault = true);
            var query = _queryBuilder.Build(out var config);
            return ExecuteFirstOrDefaultAsync<T>(query, config, token);
        }

        private async Task<CollectionSegment<T>> ExecuteToPageAsync<T>(
            string queryString,
            GraphQLRequestMetadataConfiguration config,
            CancellationToken token)
        {
            var value = await ExecuteAsync(queryString, token);
            var collectionSegment = value.Deserialize(typeof(CollectionSegment<>).MakeGenericType(config.RootType), _jsonSerializerOptions)!;
            var items = collectionSegment.GetType()
                .GetProperty(nameof(CollectionSegment<object>.Items), BindingFlags.Public | BindingFlags.Instance)!
                .GetValue(collectionSegment);
            var pageInfo = collectionSegment.GetType()
                .GetProperty(nameof(CollectionSegment<object>.PageInfo), BindingFlags.Public | BindingFlags.Instance)!
                .GetValue(collectionSegment);
            var totalCount = collectionSegment.GetType()
                .GetProperty(nameof(CollectionSegment<object>.TotalCount), BindingFlags.Public | BindingFlags.Instance)!
                .GetValue(collectionSegment);

            var evaluatedItemsArray = LocalCallChainEvaluator.EvaluateArray(items!, config.CallChain.GetEvaluateExpressions());

            var resultCollectionSegmentCtor = typeof(CollectionSegment<T>)
                .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(c => c.GetParameters().Length == 3);
            var resultCollectionSegment = resultCollectionSegmentCtor.Invoke(new object?[] { evaluatedItemsArray, pageInfo, totalCount });

            return (CollectionSegment<T>)resultCollectionSegment;
        }

        private async Task<T[]> ExecuteToArrayAsync<T>(
            string queryString,
            GraphQLRequestMetadataConfiguration config,
            CancellationToken token)
        {
            var value = await ExecuteAsync(queryString, token);
            var array = value.Deserialize(config.RootType.MakeArrayType(), _jsonSerializerOptions)!;

            var evaluatedArray = LocalCallChainEvaluator.EvaluateArray(array, config.CallChain.GetEvaluateExpressions());
            return (T[])evaluatedArray!;
        }

        private async Task<T?> ExecuteFirstOrDefaultAsync<T>(
            string queryString,
            GraphQLRequestMetadataConfiguration config,
            CancellationToken token)
        {
            var value = await ExecuteAsync(queryString, token);
            var entity = value.Deserialize(config.RootType, _jsonSerializerOptions);
            if (entity is null) return default;

            var evaluatedEntity = LocalCallChainEvaluator.EvaluateSingle(entity, config.CallChain.GetEvaluateExpressions());
            return (T?)evaluatedEntity;
        }

        private async Task<JsonElement> ExecuteAsync(
            string queryString,
            CancellationToken token)
        {
            var response = await ExecuteQueryAsync(queryString, token);

            var root = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: token);
            var hasErrors = root.TryGetProperty("errors", out var jsonErrors);

            if (hasErrors)
            {
                var errors = jsonErrors.Deserialize<GraphQLError[]>(_jsonSerializerOptions)!;

                if (errors.HasAuthorizationError() &&
                    _authorizationService is not null)
                {
                    await InvokeAuthorize(token);
                    response = await ExecuteQueryAsync(queryString, token);
                    root = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: token);
                    hasErrors = root.TryGetProperty("errors", out jsonErrors);
                }
            }

            if (hasErrors)
            {
                var errors = jsonErrors.Deserialize<GraphQLError[]>(_jsonSerializerOptions)!;
                throw new GraphQLException(errors);
            }

            response.EnsureSuccessStatusCode();

            var data = root.GetProperty("data");
            var enumerator = data.EnumerateObject();
            enumerator.MoveNext();
            var value = enumerator.Current.Value;

            return value;
        }

        private Task InvokeAuthorize(CancellationToken token)
        {
            var method = _authorizationService!.GetType()
                .GetMethods()
                .Single(m => m.Name == "Authorize" && m.GetParameters().Length == 2);

            return (Task)method.Invoke(_authorizationService, new object[] { _httpClient, token })!;
        }

        private Task<HttpResponseMessage> ExecuteQueryAsync(string queryString, CancellationToken token)
        {
            var queryObject = new
            {
                query = queryString
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(queryObject), Encoding.UTF8, "application/json")
            };

            return _httpClient.SendAsync(request, token);
        }
    }
}
