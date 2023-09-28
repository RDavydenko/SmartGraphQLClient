using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Services.Abstractions;
using SmartGraphQLClient.Errors;
using SmartGraphQLClient.Exceptions;

namespace SmartGraphQLClient.Core.GraphQLRequestExecutor.Base;

internal abstract class GraphQLRequestExecutorBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IGraphQLAuthorizationService? _authorizationService;
    private readonly HttpClient _httpClient;
    
    protected readonly JsonSerializerOptions JsonSerializerOptions =
        new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true,
        };

    protected GraphQLRequestExecutorBase(
        IServiceProvider serviceProvider,
        HttpClient httpClient,
        Type graphQLClientType)
    {
        _serviceProvider = serviceProvider;
        _authorizationService =
            (IGraphQLAuthorizationService?)_serviceProvider.GetService(
                typeof(IGraphQLAuthorizationService<>).MakeGenericType(graphQLClientType));
        _httpClient = httpClient;
    }

    protected async Task<JsonElement> ExecuteAsync(
        string queryString,
        CancellationToken token)
    {
        var response = await ExecuteQueryAsync(queryString, token);

        var root = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: token);
        var hasErrors = root.TryGetProperty("errors", out var jsonErrors);

        if (hasErrors)
        {
            var errors = jsonErrors.Deserialize<GraphQLError[]>(JsonSerializerOptions)!;

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
            var errors = jsonErrors.Deserialize<GraphQLError[]>(JsonSerializerOptions)!;
            throw new GraphQLException(errors);
        }

        response.EnsureSuccessStatusCode();

        var data = root.GetProperty("data");
        var enumerator = data.EnumerateObject();
        enumerator.MoveNext();
        var value = enumerator.Current.Value;

        return value;
    }

    protected Task InvokeAuthorize(CancellationToken token)
    {
        if (_authorizationService == null)
            return Task.CompletedTask;

        return _authorizationService.Authorize(_httpClient, token);
    }

    protected Task<HttpResponseMessage> ExecuteQueryAsync(string queryString, CancellationToken token)
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