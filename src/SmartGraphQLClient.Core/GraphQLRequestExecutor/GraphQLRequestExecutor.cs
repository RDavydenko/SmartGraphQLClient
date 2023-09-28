using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Utils;
using System.Reflection;
using System.Text.Json;
using SmartGraphQLClient.Core.GraphQLRequestExecutor.Base;

namespace SmartGraphQLClient.Core.GraphQLRequestExecutor
{
    internal class GraphQLRequestExecutor : GraphQLRequestExecutorBase
    {
        private readonly GraphQLQueryBuilder.GraphQLQueryBuilder _queryBuilder;
        
        public GraphQLRequestExecutor(
            IServiceProvider serviceProvider,
            HttpClient httpClient,
            GraphQLQueryBuilder.GraphQLQueryBuilder queryBuilder,
            Type graphQLClientType)
            : base(serviceProvider, httpClient, graphQLClientType)
        {
            _queryBuilder = queryBuilder;
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
            var collectionSegment = value.Deserialize(typeof(CollectionSegment<>).MakeGenericType(config.RootType), JsonSerializerOptions)!;
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
            var array = value.Deserialize(config.RootType.MakeArrayType(), JsonSerializerOptions)!;

            var evaluatedArray = LocalCallChainEvaluator.EvaluateArray(array, config.CallChain.GetEvaluateExpressions());
            return (T[])evaluatedArray!;
        }

        private async Task<T?> ExecuteFirstOrDefaultAsync<T>(
            string queryString,
            GraphQLRequestMetadataConfiguration config,
            CancellationToken token)
        {
            var value = await ExecuteAsync(queryString, token);
            var entity = value.Deserialize(config.RootType, JsonSerializerOptions);
            if (entity is null) return default;

            var evaluatedEntity = LocalCallChainEvaluator.EvaluateSingle(entity, config.CallChain.GetEvaluateExpressions());
            return (T?)evaluatedEntity;
        }
    }
}
