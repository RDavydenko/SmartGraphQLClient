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
        public void AllExpression_Children()
        {
            var testCases = new (string name, Expression<Func<TestEntity, bool>> expression)[]
            {
                ("childrenArray", (x) => x.ChildrenArray.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenArrayNullable", (x) => x.ChildrenArrayNullable.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenList", (x) => x.ChildrenList.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenListNullable", (x) => x.ChildrenListNullable.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenEnumerable", (x) => x.ChildrenEnumerable.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenEnumerableNullable", (x) => x.ChildrenEnumerableNullable.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenCollection", (x) => x.ChildrenCollection.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenCollectionNullable", (x) => x.ChildrenCollectionNullable.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenHashSet", (x) => x.ChildrenHashSet.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
                ("childrenHashSetNullable", (x) => x.ChildrenHashSetNullable.All(m => m.Id > 10 && m.Code.StartsWith("Sys"))),
            };

            foreach (var (name, expression) in testCases)
            {
                var expected = @$"
                    {name}: {{ 
                        all: {{ 
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
