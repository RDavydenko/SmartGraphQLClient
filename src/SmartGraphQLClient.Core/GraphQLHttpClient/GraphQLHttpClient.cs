using SmartGraphQLClient.Core.GraphQLQueryable;
using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;

namespace SmartGraphQLClient
{
    public class GraphQLHttpClient : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;

        protected HttpClient HttpClient { get; }

        public GraphQLHttpClient(
            HttpClient httpClient,
            IServiceProvider serviceProvider)
        {
            HttpClient = httpClient;
            _serviceProvider = serviceProvider;
        }

        public IGraphQLQueryable<T> Query<T>()
            => Query<T>(default!);

        public IGraphQLQueryable<T> Query<T>(string endpoint)
            => new GraphQLQueryable<T>(GetType(), _serviceProvider, HttpClient, endpoint);

        public Task<T> ExecuteRawQueryAsync<T>(string query, CancellationToken token = default)
            => new GraphQLQueryable<T>(GetType(), _serviceProvider, HttpClient).ExecuteRawQueryAsync<T>(query, token);
        
        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}
