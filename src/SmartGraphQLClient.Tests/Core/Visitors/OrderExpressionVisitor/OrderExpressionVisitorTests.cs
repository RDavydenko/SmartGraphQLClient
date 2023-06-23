using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Tests.TestsInfrastructure;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.OrderExpressionVisitor
{
    [TestClass]
    public class OrderExpressionVisitorTests : TestBase
    {
        private SmartGraphQLClient.Core.Visitors.OrderExpressionVisitor.OrderExpressionVisitor CreateVisitor(
            LambdaExpression expression, OrderDirection direction)
            => new(expression, direction, ServiceProvider);

        [TestMethod]
        public void Order_FirstLevelExpression_Asc()
        {
            Expression<Func<TestEntity, int>> expression =
                (x) => x.Id;

            var expected = "id: ASC".Tokenize();

            var visitor = CreateVisitor(expression, OrderDirection.ASC);
            visitor.Visit();

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Order_FirstLevelExpression_Desc()
        {
            Expression<Func<TestEntity, int>> expression =
                (x) => x.Id;

            var expected = "id: DESC".Tokenize();

            var visitor = CreateVisitor(expression, OrderDirection.DESC);
            visitor.Visit();

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Order_SecondLevelExpression_Asc()
        {
            Expression<Func<TestEntity, int>> expression =
                (x) => x.Child.Age;

            var expected = "child: { age: ASC }".Tokenize();

            var visitor = CreateVisitor(expression, OrderDirection.ASC);
            visitor.Visit();

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Order_SecondLevelExpression_Desc()
        {
            Expression<Func<TestEntity, int>> expression =
                (x) => x.Child.Age;

            var expected = "child: { age: DESC }".Tokenize();

            var visitor = CreateVisitor(expression, OrderDirection.DESC);
            visitor.Visit();

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
