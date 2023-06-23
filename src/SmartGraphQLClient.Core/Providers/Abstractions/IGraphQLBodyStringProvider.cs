using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.Providers.Abstractions
{
    public interface IGraphQLBodyStringProvider
    {
        (string Body, LambdaExpression Selector) Build(
            Type entityType, 
            LambdaExpression? selector, 
            List<IncludeExpressionNode> includes,
            SelectExpressionVisitorConfiguration configuration);
    }
}
