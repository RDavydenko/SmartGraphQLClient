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
        public void ContainsExpression_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.Contains("r");

            var expected = "name: { contains: \"r\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void ContainsExpression_Child_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.Type.Contains("og");

            var expected = "child: { type: { contains: \"og\" } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void ContainsExpression_String_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.Contains("ttt") && x.Id == 5;

            var expected = @"
                and: [
                    { name: { contains: ""ttt"" } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void ContainsExpression_String_With_LogicalExpression_V2()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Name.Contains("ttt") && x.Id == 5;

            var expected = @"
                and: [
                    { name: { ncontains: ""ttt"" } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void HarderContainsExpression_String_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.Contains("oma") || x.Name.Contains("iza");

            var expected = @"
                or: [
                    { name: { contains: ""oma"" } }
                    { name: { contains: ""iza"" } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NotContainsExpression_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Name.Contains("Substring");

            var expected = @"name: { ncontains: ""Substring"" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
