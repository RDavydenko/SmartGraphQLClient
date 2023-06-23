using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Providers.Abstractions;
using SmartGraphQLClient.Core.Visitors.OrderExpressionVisitor;
using System.Linq.Expressions;
using System.Text;

namespace SmartGraphQLClient.Core.Providers
{
    internal class DefaultGraphQLOrderStringProvider : IGraphQLOrderStringProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DefaultGraphQLOrderStringProvider(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string? Build(Type entityType, List<OrderExpression> orders)
        {
            const string keyword = "order";

            if (orders.Count == 0) return null;

            var sb = new StringBuilder();
            sb.AppendLine($"{keyword}: [");
            foreach (var order in orders)
            {
                sb.AppendLine($"{{ {BuildString(order.Expression, order.Direction)} }}");
            }
            sb.AppendLine(" ]");

            return sb.ToString();
        }

        private string BuildString(LambdaExpression expression, OrderDirection direction)
        {
            var visitor = new OrderExpressionVisitor(expression, direction, _serviceProvider);
            visitor.Visit();
            return visitor.ToString();
        }
    }
}
