using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Abstractions;
using System.Linq.Expressions;
using System.Text;

namespace SmartGraphQLClient.Core.Visitors.OrderExpressionVisitor
{
    internal class OrderExpressionVisitor : VisitorBase
    {
        private readonly LambdaExpression _expression;
        private readonly OrderDirection _direction;
        private StringBuilder _sb = new();

        internal OrderExpressionVisitor(
            LambdaExpression expression,
            OrderDirection direction,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _expression = expression;
            _direction = direction;
        }

        public void Visit()
        {
            _sb = new();
            Visit(_expression.Body);
        }

        private void Visit(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                var chain = ExpressionHelper.GetSequenceCallChain(memberExpression);
                VisitSequenceCallChain(chain);
            }
            else
            {
                throw new InvalidOperationException($"Expression {expression.GetType().Name} is not supported");
            }

        }

        private void VisitSequenceCallChain(List<Expression> chain)
        {
            var memberExpressions = chain.OfType<MemberExpression>().ToList();
            if (memberExpressions.Count != chain.Count)
            {
                throw new ArgumentException("All items of expression's chain must be MemberExpression");
            }
            if (memberExpressions.Count == 0)
            {
                throw new ArgumentException("Expression's chain is empty");
            }

            memberExpressions.Reverse();
            _sb.Append($"{FormatFieldName(memberExpressions[0].Member)}: {_direction}");

            foreach (var member in memberExpressions.Skip(1))
            {
                _sb.Insert(0, $"{FormatFieldName(member.Member)}: {{ ");
                _sb.Append(" }");
            }
        }

        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
