using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void SimpleEqExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == 1;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqExpression_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => 1 == x.Id;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleLessExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id < 1;

            var expected = "id: { lt: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleLessExpression_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => 1 > x.Id;

            var expected = "id: { lt: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleGreaterOrEqualExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id >= 5;

            var expected = "id: { gte: 5 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleGreaterOrEqualExpression_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => 5 <= x.Id;

            var expected = "id: { gte: 5 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqExpression_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name == "Roma";

            var expected = "name: { eq: \"Roma\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
