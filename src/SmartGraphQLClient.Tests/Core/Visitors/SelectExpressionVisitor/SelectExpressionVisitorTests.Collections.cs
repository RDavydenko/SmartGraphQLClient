using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System.Linq;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    public partial class SelectExpressionVisitorTests
    {
        [TestMethod]
        public void Select_FullCollection()
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
                    x.ChildrenArray
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection()
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
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_FromChild()
        {
            var expected = @"
                id 
                name 
                child {
                    violations { 
                        id
                        code
                    }
                }
                ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    Violations = x.Child
                        .Violations
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_SomeRequests()
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
                    ViolationIds = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                        })
                        .ToArray(),
                    ViolationCodes = x.ChildrenArray
                        .Select(m => m.Code)
                        .ToArray()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_SomeRequests_Duplicate()
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
                    ViolationIds = x.ChildrenArray
                        .Select(m => new
                        {
                            m.Id,
                            m.Code
                        })
                        .ToArray(),
                    ViolationCodes = x.ChildrenArray.Select(m => m.Code)
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_SubClassFromCollectionObject()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    headOfWorkers {
                        id
                        name
                        age
                        isActive
                    }
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
                            m.HeadOfWorkers
                        })
                        .ToArray()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_SubCollectionFromCollectionObject()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    workers {
                        id
                        name
                        age
                        isActive
                    }
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
                            m.Workers
                        })
                        .ToArray()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_SelectExpressionCollection_PartCollectionFromCollectionObject()
        {
            var expected = @"
                id 
                name 
                childrenArray { 
                    workers {
                        id
                        name
                    }
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
                            Workers = m.Workers
                                .Select(w => new
                                {
                                    w.Id,
                                    w.Name
                                })
                        })
                        .ToArray()
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
