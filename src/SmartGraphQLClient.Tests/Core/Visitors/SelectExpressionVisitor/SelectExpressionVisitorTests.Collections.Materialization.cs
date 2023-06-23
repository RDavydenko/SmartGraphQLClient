using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System.Collections.Immutable;
using System.Linq;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    public partial class SelectExpressionVisitorTests
    {
        [TestMethod]
        public void Select_SelectExpressionCollection_ToArray()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .ToArray()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_ToList()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .ToList()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_ToHashSet()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .ToHashSet()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_ToDictionary()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .ToDictionary(x => x.Id, x => x.Code)
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_ToLookup()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .ToLookup(x => x.Id, x => x.Code)
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_ToImmutableArray()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .ToImmutableArray()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_LongMethodsChain_EndsOnMethodCall()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .GroupBy(m => m.Id)
                        .Select(g => g.First())
                        .FirstOrDefault()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_LongMethodsChain_EndsOnMemberAccess()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    id
                    code
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .GroupBy(m => m.Id)
                        .Select(g => g.First())
                        .First()
                        .Code
                        .Length
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
