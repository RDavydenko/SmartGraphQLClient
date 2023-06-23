using System;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.TestsInfrastructure.Factories
{
    internal class ExpressionFactory<T>
    {
        private ExpressionFactory() { }

        public static Expression<Func<T, E>> Create<E>(Expression<Func<T, E>> expression)
            where E : class
            => expression;
    }
}
