using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure;
using System;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    [TestClass]
    public partial class WhereExpressionVisitorTests : TestBase
    {
        private const int ValueConst = 1;

        private int value = 1;

        private int Value { get; set; } = 1;

        private int GetValue() => 1;

        private SmartGraphQLClient.Core.Visitors.WhereExpressionVisitor.WhereExpressionVisitor CreateVisitor<T>(
            Expression<Func<T, bool>> expression)
            => new SmartGraphQLClient.Core.Visitors.WhereExpressionVisitor.WhereExpressionVisitor(expression, ServiceProvider);
    }
}
