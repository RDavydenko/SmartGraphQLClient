using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Providers.Abstractions;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Abstractions;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.Providers
{
    internal class DefaultGraphQLBodyStringProvider : IGraphQLBodyStringProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultGraphQLBodyStringProvider(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public (string Body, LambdaExpression Selector) Build(
            Type entityType, 
            LambdaExpression? selector, 
            List<IncludeExpressionNode> includes,
            SelectExpressionVisitorConfiguration configuration)
        {
            SelectExpressionVisitorBase visitor;

            if (selector is not null)
            {
                visitor = new SelectExpressionVisitor(selector, _serviceProvider, configuration);
            }
            else
            {
                visitor = new DefaultSelectExpressionVisitor(entityType, _serviceProvider, configuration);
            }

            visitor.Visit();
            visitor.VisitIncludeExpressions(includes);
            return (visitor.ToString(), visitor.GetSelector());
        }
    }
}
