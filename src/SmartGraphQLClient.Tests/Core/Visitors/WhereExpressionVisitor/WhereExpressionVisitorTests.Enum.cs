using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities.Enums;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void SimpleEqExpression_Enum()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.State == SomethingEnum.APPROVED;

            var expected = "state: { eq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqExpression_Enum_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => SomethingEnum.APPROVED == x.State;

            var expected = "state: { eq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleNotEqExpression_Enum()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.State != SomethingEnum.APPROVED;

            var expected = "state: { neq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleNotEqExpression_Enum_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => SomethingEnum.APPROVED != x.State;

            var expected = "state: { neq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_Enum()
        {
            var source = new[] { SomethingEnum.DELETED, SomethingEnum.NONE, SomethingEnum.DRAFT };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.State);

            var expected = "state: { in: [ DELETED, NONE, DRAFT ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_NotContainsExpression_Enum()
        {
            var source = new[] { SomethingEnum.DELETED, SomethingEnum.NONE, SomethingEnum.DRAFT };
            Expression<Func<TestEntity, bool>> expression = (x) => !source.Contains(x.State);

            var expected = "state: { nin: [ DELETED, NONE, DRAFT ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqExpression_NullableEnum()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.PreviousState == SomethingEnum.APPROVED;

            var expected = "previousState: { eq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqExpression_NullableEnum_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => SomethingEnum.APPROVED == x.PreviousState;

            var expected = "previousState: { eq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleNotEqExpression_NullableEnum()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.PreviousState != SomethingEnum.APPROVED;

            var expected = "previousState: { neq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleNotEqExpression_NullableEnum_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => SomethingEnum.APPROVED != x.PreviousState;

            var expected = "previousState: { neq: APPROVED }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_NullableEnum()
        {
            var source = new SomethingEnum?[] { SomethingEnum.DELETED, SomethingEnum.NONE, SomethingEnum.DRAFT };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.PreviousState);

            var expected = "previousState: { in: [ DELETED, NONE, DRAFT ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_NotContainsExpression_NullableEnum()
        {
            var source = new SomethingEnum?[] { SomethingEnum.DELETED, SomethingEnum.NONE, SomethingEnum.DRAFT };
            Expression<Func<TestEntity, bool>> expression = (x) => !source.Contains(x.PreviousState);

            var expected = "previousState: { nin: [ DELETED, NONE, DRAFT ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqNullExpression_NullableEnum()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.PreviousState == null;

            var expected = "previousState: { eq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleNotEqNullExpression_NullableEnum()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.PreviousState != null;

            var expected = "previousState: { neq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableEnum_HasValue()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.PreviousState.HasValue;

            var expected = "previousState: { neq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NullableEnum_NotHasValue()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => !x.PreviousState.HasValue;

            var expected = "previousState: { eq: null }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_NullableEnum_V2()
        {
            var source = new SomethingEnum?[] { SomethingEnum.DELETED, SomethingEnum.NONE, SomethingEnum.DRAFT, null };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.PreviousState);

            var expected = "previousState: { in: [ DELETED, NONE, DRAFT, null ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_NotContainsExpression_NullableEnum_V2()
        {
            var source = new SomethingEnum?[] { SomethingEnum.DELETED, SomethingEnum.NONE, SomethingEnum.DRAFT, null };
            Expression<Func<TestEntity, bool>> expression = (x) => !source.Contains(x.PreviousState);

            var expected = "previousState: { nin: [ DELETED, NONE, DRAFT, null ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
