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
        public void SimpleEqExpression_Null()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Name == null;

            var expected = "name: { eq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleNotEqExpression_Child_Null()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child != null;

            var expected = "child: { neq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleNotEqExpression_ChildField_Null()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.Summary != null;

            var expected = "child: { summary: { neq: null } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
