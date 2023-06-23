using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using System;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.TestsInfrastructure.Factories
{
    public class VisitorFactory<T>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly SelectExpressionVisitorConfiguration? _configuration;

        public VisitorFactory(
            IServiceProvider serviceProvider, 
            SelectExpressionVisitorConfiguration? configuration = null)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        internal SelectExpressionVisitor Select<TOut>(Expression<Func<T, TOut>> selector)
            => new SelectExpressionVisitor(selector, _serviceProvider, _configuration ?? new());

        internal SelectExpressionVisitor Empty()
            => new SelectExpressionVisitor(default!, _serviceProvider, _configuration ?? new());
    }
}
