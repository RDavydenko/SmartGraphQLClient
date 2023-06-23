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
        public void SimpleAndExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == 1 && x.Name == "Roma";

            var expected = @"
                and: [ 
                    { id: { eq: 1 } } 
                    { name: { eq: ""Roma"" } }
                ]"
                .Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void HarderAndExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == 1 && x.Name == "Roma" && x.Id < 20;

            var expected = @"
                and: [
                    { and: [
                        { id: { eq: 1 } }
                        { name: { eq: ""Roma"" } }
                    ] }
                    { id: { lt: 20 } }
                ]"
                .Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleOrExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == 1 || x.Name == "Roma";

            var expected = @"
                or: [ 
                    { id: { eq: 1 } } 
                    { name: { eq: ""Roma"" } }
                ]"
                .Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void HarderOrExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == 1 || x.Name == "Roma" || x.Id < 20;

            var expected = @"
                or: [
                    { or: [
                        { id: { eq: 1 } }
                        { name: { eq: ""Roma"" } }
                    ] }
                    { id: { lt: 20 } }
                ]"
                .Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void AndWithOrExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id == 1 && x.Name == "Roma" || x.Id < 20;

            var expected = @"
                or: [
                    { and: [
                        { id: { eq: 1 } }
                        { name: { eq: ""Roma"" } }
                    ] }
                    { id: { lt: 20 } }
                ]"
                .Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void AndWithOrExpression_Reversed()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id < 20 || x.Id == 1 && x.Name == "Roma";

            var expected = @"
                or: [
                    { id: { lt: 20 } }
                    { and: [
                        { id: { eq: 1 } }
                        { name: { eq: ""Roma"" } }
                    ] }
                ]"
                .Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void HarderAndWithOrExpression()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => (x.Name != "Dog" && x.Id < 20) || (x.Id == 1 && x.Name == "Roma");

            var expected = @"
                or: [
                    { and: [
                        { name: { neq: ""Dog"" } }
                        { id: { lt: 20 } }
                    ] }
                    { and: [
                        { id: { eq: 1 } }
                        { name: { eq: ""Roma"" } }
                    ] }
                ]"
                .Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.Age == 1;

            var expected = "child: { age: { eq: 1 } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleEqExpression_ChildNullable()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.ChildNullable.Age == 1;

            var expected = "childNullable: { age: { eq: 1 } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void SimpleAndExpression_Child()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Child.Age == 1 && x.Child.Type == "Cat";

            var expected = @"
                and: [
                    { child: { age: { eq: 1 } } }
                    { child: { type: { eq: ""Cat"" } } }
                ]
            ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void CombinedExpression_Child_with_Parent()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => x.Id > 10 || x.Child.Age == 1 && x.Child.Type == "Cat";

            var expected = @"
                or: [
                    { id: { gt: 10 } }
                    { and: [
                        { child: { age: { eq: 1 } } }
                        { child: { type: { eq: ""Cat"" } } }
                    ] }   
                ]
            ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
