using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    public partial class SelectExpressionVisitorTests
    {
        [TestMethod]
        public void Select_AnonymousObject()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_DuplicateFields()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    DulpicateName = x.Name
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_MethodCallWithField()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    DoubledId = x.Id * 2,
                    IsNameStartWithALetter = x.Name.StartsWith("A"),
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_MethodCallsChainWithField()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    DoubledId = x.Id * 2,
                    Chain = x.Name.ToLower().ToUpper().Replace("A", "B"),
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_MethodCallWithField_WithUnary()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    DoubledId = x.Id * 2,
                    IsNotNameStartWithALetter = !x.Name.StartsWith("A"),
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_MethodCallWithField_WithBinary()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    DoubledId = x.Id * 2,
                    IsNameStartWithALetter = x.Name.StartsWith("A") != true,
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
        [TestMethod]
        public void Select_AnonymousObject_BinaryExpressionWithField()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    DoubledId = x.Id * 2,
                    SysName = x.Name + "_Sys",
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithFieldAliases()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    AlternativeId = x.Id,
                    AlternativeName = x.Name,
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_NewClass()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new TestEntity
                {
                    Id = x.Id,
                    Name = x.Name,
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_NewClass_WithParametricConstructor()
        {
            var expected = "id name".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new TestEntity(x.Id, x.Name)
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithParameterExpression()
        {
            var expected = @"
                id
                maybeId
                name
                array
                isStarted
                isCompleted
                state
                previousState
                createDate
                updateDate
                ".Tokenize();
            var visitor = VisitorFactory<TestEntity>()
                .Select((x) => new { Entity = x });

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
