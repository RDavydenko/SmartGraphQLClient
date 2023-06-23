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
        public void StartsWithExpression_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.StartsWith("r");

            var expected = "name: { startsWith: \"r\" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void StartsWithExpression_Child_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.Type.StartsWith("og");

            var expected = "child: { type: { startsWith: \"og\" } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void StartsWithExpression_String_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.StartsWith("ttt") && x.Id == 5;

            var expected = @"
                and: [
                    { name: { startsWith: ""ttt"" } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void StartsWithExpression_String_With_LogicalExpression_V2()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Name.StartsWith("ttt") && x.Id == 5;

            var expected = @"
                and: [
                    { name: { nstartsWith: ""ttt"" } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void HarderStartsWithExpression_String_With_LogicalExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name.StartsWith("oma") || x.Name.StartsWith("iza");

            var expected = @"
                or: [
                    { name: { startsWith: ""oma"" } }
                    { name: { startsWith: ""iza"" } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NotStartsWithExpression_String()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Name.StartsWith("Substring");

            var expected = @"name: { nstartsWith: ""Substring"" }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
