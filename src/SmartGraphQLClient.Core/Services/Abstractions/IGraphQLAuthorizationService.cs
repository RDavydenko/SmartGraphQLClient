namespace SmartGraphQLClient.Core.Services.Abstractions
{
    public interface IGraphQLAuthorizationService<out TGraphQLHttpClient> : IGraphQLAuthorizationService
        where TGraphQLHttpClient : GraphQLHttpClient
    {
    }

    public interface IGraphQLAuthorizationService
    {
        Task Authorize(HttpClient httpClient, CancellationToken cancellationToken);
    }
}
