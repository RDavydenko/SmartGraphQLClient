using SmartGraphQLClient.Core.Models.Internal;

namespace SmartGraphQLClient.Core.Providers.Abstractions
{
    public interface IGraphQLOrderStringProvider
    {
        string? Build(Type entityType, List<OrderExpression> orders);
    }
}
