namespace SmartGraphQLClient.Core.Services.Abstractions
{
    public interface IGraphQLAuthorizationService<out TGraphQLHttpClient>
        where TGraphQLHttpClient : GraphQLHttpClient
    {
        Task Authorize(HttpClient httpClient, CancellationToken cancellationToken);
    }
}
