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
        public void BooleanEqExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.IsStarted;

            var expected = @"isStarted: { eq: true }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void BooleanNotEqExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.IsStarted;

            var expected = @"isStarted: { neq: true }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void BooleanEqualTrueExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.IsStarted == true;

            var expected = @"isStarted: { eq: true }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void BooleanNotEqualTrueExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.IsStarted != true;

            var expected = @"isStarted: { neq: true }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableBooleanEqualTrueExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.IsCompleted == true;

            var expected = @"isCompleted: { eq: true }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableBooleanEqualNullExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.IsCompleted == null;

            var expected = @"isCompleted: { eq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void BooleanEqExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.IsStarted;

            var expected = @"child: { isStarted: { eq: true } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void BooleanNotEqExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.Child.IsStarted;

            var expected = @"child: { isStarted: { neq: true } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void BooleanEqualTrueExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.IsStarted == true;

            var expected = @"child: { isStarted: { eq: true } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void BooleanNotEqualTrueExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.IsStarted != true;

            var expected = @"child: { isStarted: { neq: true } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableBooleanEqualTrueExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.IsCompleted == true;

            var expected = @"child: { isCompleted: { eq: true } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableBooleanEqualNullExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.IsCompleted == null;

            var expected = @"child: { isCompleted: { eq: null } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableBooleanExpression_HasValue()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.IsCompleted.HasValue;

            var expected = @"isCompleted: { neq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableBooleanExpression_Not_HasValue()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.IsCompleted.HasValue;

            var expected = @"isCompleted: { eq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
