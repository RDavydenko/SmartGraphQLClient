using SmartGraphQLClient.Core.Models.Internal;

namespace SmartGraphQLClient.Core.GraphQLQueryBuilder.Models
{
    internal class GraphQLQueryBuilderConfiguration
    {
        public GraphQLQueryBuilderConfiguration(
            Type rootEntityType,
            GraphQLExpressionCallChainConfiguration callChain,
            IReadOnlyDictionary<string, object?> configuration,
            IServiceProvider serviceProvider,
            GraphQLRequestConfiguration? requestConfiguration)
        {
            RootEntityType = rootEntityType;
            CallChain = callChain;
            QueryableConfiguration = configuration;

            ServiceProvider = serviceProvider;
            RequestConfiguration = requestConfiguration ?? new();
        }

        public Type RootEntityType { get; }
        public GraphQLExpressionCallChainConfiguration CallChain { get; }
        public IReadOnlyDictionary<string, object?> QueryableConfiguration { get; } = new Dictionary<string, object?>();

        public IServiceProvider ServiceProvider { get; }
        public GraphQLRequestConfiguration RequestConfiguration { get; }
    }
}
