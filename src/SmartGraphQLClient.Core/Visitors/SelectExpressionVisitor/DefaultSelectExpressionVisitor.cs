using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Abstractions;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor
{
    internal class DefaultSelectExpressionVisitor : SelectExpressionVisitorBase
    {
        private static MethodInfo GetDefaultSelectorMethod =
            typeof(DefaultSelectExpressionVisitor)
                .GetMethod(nameof(GetDefaultSelector), BindingFlags.Static | BindingFlags.NonPublic)!;

        internal DefaultSelectExpressionVisitor(
            Type rootType,
            IServiceProvider serviceProvider,
            SelectExpressionVisitorConfiguration configuration)
            : base(serviceProvider, configuration)
        {
            _rootType = rootType;
        }

        public override void Visit()
        {
            _root = new("");
            VisitAllSimpleMembers(_rootType, _root);
        }

        internal override LambdaExpression GetSelector()
        {
            return (LambdaExpression)GetDefaultSelectorMethod.MakeGenericMethod(_rootType).Invoke(null, null)!;
        }

        private static Expression<Func<T, T>> GetDefaultSelector<T>()
            => (x) => x;
    }
}
