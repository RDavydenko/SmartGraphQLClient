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
        public void EndsWithExpression_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.EndsWith("r");

            var expected = "name: { endsWith: \"r\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EndsWithExpression_Child_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.Type.EndsWith("og");

            var expected = "child: { type: { endsWith: \"og\" } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EndsWithExpression_String_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.EndsWith("ttt") && x.Id == 5;

            var expected = @"
                and: [
                    { name: { endsWith: ""ttt"" } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EndsWithExpression_String_With_LogicalExpression_V2()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Name.EndsWith("ttt") && x.Id == 5;

            var expected = @"
                and: [
                    { name: { nendsWith: ""ttt"" } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void HarderEndsWithExpression_String_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.EndsWith("oma") || x.Name.EndsWith("iza");

            var expected = @"
                or: [
                    { name: { endsWith: ""oma"" } }
                    { name: { endsWith: ""iza"" } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NotEndsWithExpression_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Name.EndsWith("Substring");

            var expected = @"name: { nendsWith: ""Substring"" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
