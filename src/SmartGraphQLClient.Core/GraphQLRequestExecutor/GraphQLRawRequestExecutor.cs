using System.Text.Json;
using SmartGraphQLClient.Core.GraphQLRequestExecutor.Base;

namespace SmartGraphQLClient.Core.GraphQLRequestExecutor;

internal class GraphQLRawRequestExecutor : GraphQLRequestExecutorBase
{
    public GraphQLRawRequestExecutor(
        IServiceProvider serviceProvider, 
        HttpClient httpClient, 
        Type graphQLClientType)
        : base(serviceProvider, httpClient, graphQLClientType)
    {
    }
    
    public async Task<T> ExecuteRawQueryAsync<T>(string query, CancellationToken token)
    {
        var value = await ExecuteAsync(query, token);
        return value.Deserialize<T>(JsonSerializerOptions)!;
    }
}