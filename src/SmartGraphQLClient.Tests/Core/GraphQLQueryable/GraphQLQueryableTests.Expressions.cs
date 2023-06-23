using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Extensions;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using System.Linq;

namespace SmartGraphQLClient.Tests.Core.GraphQLQueryable
{
    public partial class GraphQLQueryableTests
    {
        [TestMethod]
        public void BuildQuery_Select()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Select(x => new
                {
                    x.Id,
                    Heads = x.ChildrenArray
                        .Select(m => m.HeadOfWorkers)
                        .ToArray()
                });

            var expected = @"
                {
                    entities {
                        id
                        childrenArray {
                            headOfWorkers {
                                id
                                name
                                age
                                isActive
                            }
                        }
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Include()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Include(x => x.Child);

            var expected = @"
                {
                    entities {
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
                        child {
                            age
                            type
                            summary
                            array
                            isStarted
                            isCompleted
                        }
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Include_Include()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Include(x => x.Child)
                .Include(x => x.ChildrenEnumerable);

            var expected = @"
                {
                    entities {
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
                        child {
                            age
                            type
                            summary
                            array
                            isStarted
                            isCompleted
                        }
                        childrenEnumerable {
                            id
                            code
                        }
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Include_ThenInclude_ThenInclude_ThenInclude_Include_ThenInclude_ThenInclude_Include_ThenInclude_ThenInclude()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Include(x => x.Child)
                    .ThenInclude(x => x.SubChild)
                        .ThenInclude(x => x.Violations)
                            .ThenInclude(x => x.HeadOfWorkers)
                .Include(x => x.Child)
                    .ThenInclude(x => x.SubChild)
                        .ThenInclude(x => x.SubChild)
                .Include(x => x.ChildrenEnumerable)
                    .ThenInclude(x => x.Workers)
                        .ThenInclude(x => x.Chief);

            var expected = @"
                {
                    entities {                        
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
                        child { 
                            age 
                            type 
                            summary 
                            array 
                            isStarted 
                            isCompleted 
                            subChild { 
                                age 
                                type 
                                summary 
                                array 
                                isStarted 
                                isCompleted 
                                violations { 
                                    id 
                                    code 
                                    headOfWorkers { 
                                        id 
                                        name 
                                        age 
                                        isActive 
                                    } 
                                } 
                                subChild { 
                                    age 
                                    type 
                                    summary 
                                    array 
                                    isStarted 
                                    isCompleted 
                                } 
                            } 
                        } 
                        childrenEnumerable { 
                            id 
                            code 
                            workers { 
                                id 
                                name 
                                age 
                                isActive 
                                chief { 
                                    id 
                                    name 
                                    age 
                                    isActive 
                                } 
                            } 
                        }
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Where()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Where(x => x.Id > 0 && x.Child != null);

            var expected = @"
                {
                    entities (
                        where: { 
                            and: [
                                { id: { gt: 0 } }
                                { child: { neq: null } }
                            ]
                        }
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Where_Select()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Where(x => x.Id > 0 && x.Child != null)
                .Select(x => new
                {
                    x.Id,
                    Heads = x.ChildrenArray
                        .Select(m => m.HeadOfWorkers)
                        .ToArray()
                });

            var expected = @"
                {
                    entities (
                        where: { 
                            and: [
                                { id: { gt: 0 } }
                                { child: { neq: null } }
                            ]
                        }
                    ) {
                        id
                        childrenArray {
                            headOfWorkers {
                                id
                                name
                                age
                                isActive
                            }
                        }
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_2Where_Select()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Where(x => x.Id > 0 && x.Child != null)
                .Where(x => x.ChildrenArray.Any(m => m.Code != "REJECTED" || m.HeadOfWorkers.IsActive))
                .Select(x => new
                {
                    x.Id,
                    Heads = x.ChildrenArray
                        .Select(m => m.HeadOfWorkers)
                        .ToArray()
                });

            var expected = @"
                {
                    entities (
                        where: { 
                            and: [
                                {
                                    and: [
                                        { id: { gt: 0 } }
                                        { child: { neq: null } }
                                    ]
                                }
                                {
                                    childrenArray: { 
                                        some: { 
                                            or: [
                                                { code: { neq: ""REJECTED"" } }
                                                { headOfWorkers: { isActive: { eq: true } } }
                                            ]
                                        }
                                    }
                                }
                            ]
                        }
                    ) {
                        id
                        childrenArray {
                            headOfWorkers {
                                id
                                name
                                age
                                isActive
                            }
                        }
                    }
                }
            ".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_2Where_Select_Skip_Take()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Where(x => x.Id > 0 && x.Child != null)
                .Where(x => x.ChildrenArray.Any(m => m.Code != "REJECTED" || m.HeadOfWorkers.IsActive))
                .Select(x => new
                {
                    x.Id,
                    Heads = x.ChildrenArray
                        .Select(m => m.HeadOfWorkers)
                        .ToArray()
                })
                .Skip(100)
                .Take(20);

            var expected = @"
                {
                    entities (
                        where: { 
                            and: [
                                {
                                    and: [
                                        { id: { gt: 0 } }
                                        { child: { neq: null } }
                                    ]
                                }
                                {
                                    childrenArray: { 
                                        some: { 
                                            or: [
                                                { code: { neq: ""REJECTED"" } }
                                                { headOfWorkers: { isActive: { eq: true } } }
                                            ]
                                        }
                                    }
                                }
                            ]
                        }
                        skip: 100
                        take: 20
                    ) {
                        id
                        childrenArray {
                            headOfWorkers {
                                id
                                name
                                age
                                isActive
                            }
                        }
                    }
                }
            ".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Skip()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Skip(10);

            var expected = @"
                {
                    entities (
                        skip: 10
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_OrderBy()
        {
            var queryable = CreateQueryable<TestEntity>()
                .OrderBy(x => x.Id);

            var expected = @"
                {
                    entities (
                        order: [
                            { id: ASC }
                        ]
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_OrderBy_Child()
        {
            var queryable = CreateQueryable<TestEntity>()
                .OrderBy(x => x.Child.IsStarted);

            var expected = @"
                {
                    entities (
                        order: [
                            { child: { isStarted: ASC } }
                        ]
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_OrderByDescending()
        {
            var queryable = CreateQueryable<TestEntity>()
                .OrderByDescending(x => x.Id);

            var expected = @"
                {
                    entities (
                        order: [
                            { id: DESC }
                        ]
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_OrderByDescending_Child()
        {
            var queryable = CreateQueryable<TestEntity>()
                .OrderByDescending(x => x.Child.IsStarted);

            var expected = @"
                {
                    entities (
                        order: [
                            { child: { isStarted: DESC } }
                        ]
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_OrderBy_ThenBy_ThenByDescending()
        {
            var queryable = CreateQueryable<TestEntity>()
                .OrderBy(x => x.Id)
                    .ThenBy(x => x.MaybeId)
                        .ThenByDescending(x => x.Child.Summary);

            var expected = @"
                {
                    entities (
                        order: [
                            { id: ASC }
                            { maybeId: ASC }
                            { child: { summary: DESC } }
                        ]
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Where_ThenBy_ThenByDescending_Select()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Where(x => x.Name.StartsWith("R"))
                .OrderBy(x => x.Id)
                    .ThenBy(x => x.MaybeId)
                        .ThenByDescending(x => x.Child.Summary)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    ChildInfo = new
                    {
                        x.Child.IsStarted,
                        x.Child.IsCompleted,
                    }
                })
                .Skip(100)
                .Take(10);

            var expected = @"
                {
                    entities (
                        where: { name: { startsWith: ""R"" } }
                        order: [
                            { id: ASC }
                            { maybeId: ASC }
                            { child: { summary: DESC } }
                        ]
                        skip: 100
                        take: 10
                    ) {
                        id
                        name
                        child {
                            isStarted
                            isCompleted
                        }
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_Take()
        {
            var queryable = CreateQueryable<TestEntity>()
                .Take(10);

            var expected = @"
                {
                    entities (
                        take: 10
                    ) {
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
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void BuildQuery_DisableIgnoreAttributes()
        {
            var queryable = CreateQueryable<TestEntity>()
                .DisableIgnoreAttributes();

            var expected = @"
                {
                    entities {
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
                        anotherSimpleProperty
                    }
                }".Tokenize();

            var tokens = queryable.GetDebugView().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
