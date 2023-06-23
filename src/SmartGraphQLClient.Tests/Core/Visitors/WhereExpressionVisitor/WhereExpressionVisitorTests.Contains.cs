using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void Source_ContainsExpression_Int()
        {
            var source = new[] { 1, 2, 3, 4 };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.Id);

            var expected = "id: { in: [ 1, 2, 3, 4 ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_Int_Inline()
        {
            Expression<Func<TestEntity, bool>> expression = (x) => new[] { 1, 2, 3, 4 }.Contains(x.Id);

            var expected = "id: { in: [ 1, 2, 3, 4 ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_Child_Int()
        {
            var source = new HashSet<int> { 1, 2, 3, 4 };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.Child.Age);

            var expected = "child: { age: { in: [ 1, 2, 3, 4 ] } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_Int_With_LogicalExpression()
        {
            var source = new[] { 1, 2, 3, 4 }.AsEnumerable();
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.Id) && x.Id == 5;

            var expected = @"
                and: [
                    { id: { in: [ 1, 2, 3, 4 ] } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_HarderContainsExpression_Int_With_LogicalExpression()
        {
            var source1 = new[] { 1, 2, 3, 4 }.Where(x => x < int.MaxValue).Select(x => x * 2).Select(x => x / 2);
            var source2 = new List<int> { 1, 2 };

            Expression<Func<TestEntity, bool>> expression = (x) => source1.Contains(x.Id) || source2.Contains(x.Id);

            var expected = @"
                or: [
                    { id: { in: [ 1, 2, 3, 4 ] } }
                    { id: { in: [ 1, 2 ] } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_NotContainsExpression_Int()
        {
            var source = new ConcurrentBag<int> { 1, 2, 3, 4 };
            Expression<Func<TestEntity, bool>> expression = (x) => !source.Contains(x.Id);

            // reversed order
            var expected = @"id: { nin: [ 4, 3, 2, 1 ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_NotContainsExpression_Int_V2()
        {
            var source = new[] { 1, 2, 3, 4 };
            Expression<Func<TestEntity, bool>> expression = (x) => !source.Contains(x.Id);

            // reversed order
            var expected = @"id: { nin: [ 1, 2, 3, 4 ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_NullableInt()
        {
            var source = new int?[] { 1, 2, 3, null };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.MaybeId);

            var expected = @"maybeId: { in: [ 1, 2, 3, null ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }


        [TestMethod]
        public void Source_ContainsExpression_String()
        {
            var source = new[] { "1", "2", "3", "4" };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.Name);

            var expected = @"name: { in: [ ""1"", ""2"", ""3"", ""4"" ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_Child_String()
        {
            var source = new HashSet<string> { "1", "2", "3", "4" };
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.Child.Type);

            var expected = @"child: { type: { in: [ ""1"", ""2"", ""3"", ""4"" ] } }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_ContainsExpression_String_With_LogicalExpression()
        {
            var source = new[] { "1", "2", "3", "4" }.AsEnumerable();
            Expression<Func<TestEntity, bool>> expression = (x) => source.Contains(x.Name) && x.Id == 5;

            var expected = @"
                and: [
                    { name: { in: [ ""1"", ""2"", ""3"", ""4"" ] } }
                    { id: { eq: 5 } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_HarderContainsExpression_String_With_LogicalExpression()
        {
            var source1 = new[] { "1", "2", "3", "4" }.Where(x => x.Length < int.MaxValue).Select(x => x + "1").Select(x => x[..(x.Length - 1)]);
            var source2 = new List<string> { "1", "2" };

            Expression<Func<TestEntity, bool>> expression = (x) => source1.Contains(x.Name) || source2.Contains(x.Name);

            var expected = @"
                or: [
                    { name: { in: [ ""1"", ""2"", ""3"", ""4"" ] } }
                    { name: { in: [ ""1"", ""2"" ] } }
                ]
                ".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_NotContainsExpression_String()
        {
            var source = new ConcurrentBag<string> { "1", "2", "3", "4" };
            Expression<Func<TestEntity, bool>> expression = (x) => !source.Contains(x.Name);

            // reversed order
            var expected = @"name: { nin: [ ""4"", ""3"", ""2"", ""1"" ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void Source_NotContainsExpression_String_V2()
        {
            var source = new[] { "1", "2", "3", "4" };
            Expression<Func<TestEntity, bool>> expression = (x) => !source.Contains(x.Name);

            // reversed order
            var expected = @"name: { nin: [ ""1"", ""2"", ""3"", ""4"" ] }".Tokenize();

            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }
    }
}
