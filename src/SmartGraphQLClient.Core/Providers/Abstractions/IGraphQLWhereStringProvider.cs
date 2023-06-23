using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.Providers.Abstractions
{
    public interface IGraphQLWhereStringProvider
    {
        string? Build(Type entityType, List<LambdaExpression> predicates);
    }
}
