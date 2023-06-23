using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void ContainsExpression_Array()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Array.Contains(1);

            var expected = "array: { contains: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void ContainsExpression_Child_Array()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.Array.Contains(1);

            var expected = "child: { array: { contains: 1 } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void ContainsExpression_Array_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Array.Contains(1) && x.Id == 5;

            var expected = @"
                and: [
                    { array: { contains: 1 } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void HarderContainsExpression_Array_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Array.Contains(1) || x.Array.Contains(5);

            var expected = @"
                or: [
                    { array: { contains: 1 } }
                    { array: { contains: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NotContainsExpression_Array()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Array.Contains(1);

            var expected = @"array: { ncontains: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
