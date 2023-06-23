using Microsoft.VisualStudio.TestPlatform.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Tests.Core.Visitors.WhereExpressionVisitor
{
    public partial class WhereExpressionVisitorTests
    {
        [TestMethod]
        public void AnyExpression_Children()
        {
            var testCases = new (string name, Expression<Func<TestEntity, bool>> expression)[]
            {
                ("childrenArray", (x) => x.ChildrenArray.Any()),
                ("childrenArrayNullable", (x) => x.ChildrenArrayNullable.Any()),
                ("childrenList", (x) => x.ChildrenList.Any()),
                ("childrenListNullable", (x) => x.ChildrenListNullable.Any()),
                ("childrenEnumerable", (x) => x.ChildrenEnumerable.Any()),
                ("childrenEnumerableNullable", (x) => x.ChildrenEnumerableNullable.Any()),
                ("childrenCollection", (x) => x.ChildrenCollection.Any()),
                ("childrenCollectionNullable", (x) => x.ChildrenCollectionNullable.Any()),
                ("childrenHashSet", (x) => x.ChildrenHashSet.Any()),
                ("childrenHashSetNullable", (x) => x.ChildrenHashSetNullable.Any()),
            };

            foreach (var (name, expression) in testCases)
            {
                var expected = $"{name}: {{ any: true }}".Tokenize();
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void NotAnyExpression_Children()
        {
            var testCases = new (string name, Expression<Func<TestEntity, bool>> expression)[]
            {
                ("childrenArray", (x) => !x.ChildrenArray.Any()),
                ("childrenArrayNullable", (x) => !x.ChildrenArrayNullable.Any()),
                ("childrenList", (x) => !x.ChildrenList.Any()),
                ("childrenListNullable", (x) => !x.ChildrenListNullable.Any()),
                ("childrenEnumerable", (x) => !x.ChildrenEnumerable.Any()),
                ("childrenEnumerableNullable", (x) => !x.ChildrenEnumerableNullable.Any()),
                ("childrenCollection", (x) => !x.ChildrenCollection.Any()),
                ("childrenCollectionNullable", (x) => !x.ChildrenCollectionNullable.Any()),
                ("childrenHashSet", (x) => !x.ChildrenHashSet.Any()),
                ("childrenHashSetNullable", (x) => !x.ChildrenHashSetNullable.Any()),
            };

            foreach (var (name, expression) in testCases)
            {
                var expected = $"{name}: {{ any: false }}".Tokenize();
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void SomeExpression_Children()
        {
            var testCases = new (string name, Expression<Func<TestEntity, bool>> expression)[]
           {
                ("childrenArray", (x) => x.ChildrenArray.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenArrayNullable", (x) => x.ChildrenArrayNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenList", (x) => x.ChildrenList.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenListNullable", (x) => x.ChildrenListNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenEnumerable", (x) => x.ChildrenEnumerable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenEnumerableNullable", (x) => x.ChildrenEnumerableNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenCollection", (x) => x.ChildrenCollection.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenCollectionNullable", (x) => x.ChildrenCollectionNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenHashSet", (x) => x.ChildrenHashSet.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenHashSetNullable", (x) => x.ChildrenHashSetNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
            };

            foreach (var (name, expression) in testCases)
            {
                var expected = @$"
                    {name}: {{ 
                        some: {{ 
                            and: [
                                {{ id: {{ gt: 10 }} }}
                                {{ code: {{ startsWith: ""Sys"" }} }}
                            ]                            
                        }}
                    }}".Tokenize();
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }

        [TestMethod]
        public void SomeExpression_Children_Complex()
        {
            Expression<Func<TestEntity, bool>> expression =
                (x) => x.ChildrenArray.Any(m => m.Id > 10) &&
                       x.ChildrenArray.Any(m => m.HeadOfWorkers != null || !m.Workers.Any(w => !w.IsActive || w.Id == 2));

            var expected = @"
                and: [
                    { childrenArray: { some: { id: { gt: 10 } } } }
                    {
                      childrenArray: {
                        some: {
                          or: [
                            { headOfWorkers: { neq: null } }
                            {
                              workers: {
                                none: {
                                  or: [
                                    { isActive: { neq: true } } 
                                    { id: { eq: 2 } }
                                  ]
                                }
                              }
                            }
                          ]
                        }
                      }
                    }
                  ]".Tokenize();
            var visitor = CreateVisitor(expression);
            visitor.Visit();
            var result = visitor.ToString();
            var tokens = result.Tokenize();

            CollectionAssert.AreEqual(tokens, expected);
        }

        [TestMethod]
        public void NoneExpression_Children()
        {
            var testCases = new (string name, Expression<Func<TestEntity, bool>> expression)[]
           {
                ("childrenArray", (x) => !x.ChildrenArray.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenArrayNullable", (x) => !x.ChildrenArrayNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenList", (x) => !x.ChildrenList.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenListNullable", (x) => !x.ChildrenListNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenEnumerable", (x) => !x.ChildrenEnumerable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenEnumerableNullable", (x) => !x.ChildrenEnumerableNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenCollection", (x) => !x.ChildrenCollection.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenCollectionNullable", (x) => !x.ChildrenCollectionNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenHashSet", (x) => !x.ChildrenHashSet.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenHashSetNullable", (x) => !x.ChildrenHashSetNullable.Any(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
            };

            foreach (var (name, expression) in testCases)
            {
                var expected = @$"
                    {name}: {{ 
                        none: {{ 
                            and: [
                                {{ id: {{ gt: 10 }} }}
                                {{ code: {{ startsWith: ""Sys"" }} }}
                            ]                            
                        }}
                    }}".Tokenize();
                var visitor = CreateVisitor(expression);
                visitor.Visit();
                var tokens = visitor.ToString().Tokenize();

                CollectionAssert.AreEqual(tokens, expected);
            }
        }
    }
}
