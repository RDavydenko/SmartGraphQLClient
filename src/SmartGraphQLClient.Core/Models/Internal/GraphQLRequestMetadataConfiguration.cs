using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.Models.Internal
{
    internal class GraphQLRequestMetadataConfiguration
    {
        public GraphQLRequestMetadataConfiguration(
            Type rootType,
            LambdaExpression rootSelector,
            GraphQLExpressionCallChainConfiguration callChain)
        {
            RootType = rootType;
            RootSelectExpression = rootSelector;
            CallChain = callChain;
        }

        public Type RootType { get; }
        public LambdaExpression RootSelectExpression { get; }
        public GraphQLExpressionCallChainConfiguration CallChain { get; }
    }
}
