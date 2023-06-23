using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void EqExpression_With_InlineClosure()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == 1;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_LocalClosure()
        {
            var value = 1;
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == value;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_ThisInstanceMethod_Inline()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == GetValue();

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_ThisInstanceProperty_Inline()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == Value;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_ThisInstanceField_Inline()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == value;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_ThisInstanceConstant_Inline()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == ValueConst;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_OtherInstanceMethod_Inline()
        {
            var instance = new TestClosureClass(1);
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == instance.GetValue();

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_OtherInstanceProperty_Inline()
        {
            var instance = new TestClosureClass(1);
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == instance.Value;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_OtherInstanceProperty_Inline_New()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == new TestClosureClass(1).Value;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_OtherInstanceFeild_Inline()
        {
            var instance = new TestClosureClass(1);
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == instance.value;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_OtherStaticFeild_Inline()
        {
            TestClosureClass.ValueStatic = 1;
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == TestClosureClass.ValueStatic;

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_OtherStaticMethod_Inline()
        {
            TestClosureClass.ValueStatic = 1;
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == TestClosureClass.GetValueStatic();

            var expected = "id: { eq: 1 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_ParametrizedMethod_Inline()
        {
            var instance = new TestClosureClass(1);
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == instance.GetCaluclatedValue(1, 2, 3);

            var expected = "id: { eq: 6 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_ParametrizedMethod_Inline_New()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == new TestClosureClass(1).GetCaluclatedValue(1, 2, 3);

            var expected = "id: { eq: 6 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void EqExpression_With_ParametrizedMethod_Inline_V2()
        {
            var instance = new TestClosureClass(1);
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == instance.GetCaluclatedValue(instance.GetCalulatedValue(1, 2), 2, instance.Value);

            var expected = "id: { eq: 6 }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
