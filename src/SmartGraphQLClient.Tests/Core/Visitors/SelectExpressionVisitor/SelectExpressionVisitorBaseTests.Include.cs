using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Tests.TestsInfrastructure;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;
using SmartGraphQLClient.Tests.TestsInfrastructure.Factories;
using System.Collections.Generic;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    [TestClass]
    public class SelectExpressionVisitorBaseTests : TestBase
    {
        [TestMethod]
        public void SingleInclude_Class()
        {
            var expected = @"
                child {
                    age
                    type
                    summary
                    array
                    isStarted
                    isCompleted
                }
            ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Empty();
            var includeChildExpression = CreateInclude(
                ExpressionFactory<TestEntity>.Create(x => x.Child)
            );

            visitor.VisitIncludeExpressions(new() { includeChildExpression });

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void SingleInclude_ClassNullable()
        {
            var expected = @"
                childNullable {
                    age
                    type
                    summary
                    array
                    isStarted
                    isCompleted
                }
            ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Empty();
            var includeChildExpression = CreateInclude(
                ExpressionFactory<TestEntity>.Create(x => x.ChildNullable)
            );

            visitor.VisitIncludeExpressions(new() { includeChildExpression });

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void SingleInclude_CollectionClass()
        {
            var expected = @"
                childrenArray {
                    id
                    code
                }
            ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Empty();
            var includeChildExpression = CreateInclude(
                ExpressionFactory<TestEntity>.Create(x => x.ChildrenArray)
            );

            visitor.VisitIncludeExpressions(new() { includeChildExpression });

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void SingleInclude_NullableCollectionClass()
        {
            var expected = @"
                childrenCollectionNullable {
                    id
                    code
                }
            ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Empty();
            var includeChildExpression = CreateInclude(
                ExpressionFactory<TestEntity>.Create(x => x.ChildrenCollectionNullable)
            );

            visitor.VisitIncludeExpressions(new() { includeChildExpression });

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void TwoIncludes_TwoClasses()
        {
            var expected = @"
                child {
                    age
                    type
                    summary
                    array
                    isStarted
                    isCompleted
                }
                childNullable {
                    age
                    type
                    summary
                    array
                    isStarted
                    isCompleted
                }
            ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Empty();
            var includes = new List<IncludeExpressionNode>()
            {
                CreateInclude(ExpressionFactory<TestEntity>.Create(x => x.Child)),
                CreateInclude(ExpressionFactory<TestEntity>.Create(x => x.ChildNullable))
            };

            visitor.VisitIncludeExpressions(includes);

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void TwoIncludes_ClassAndCollection()
        {
            var expected = @"
                child {
                    age
                    type
                    summary
                    array
                    isStarted
                    isCompleted
                }
                childrenCollection {
                    id
                    code
                }
            ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Empty();
            var includes = new List<IncludeExpressionNode>()
            {
                CreateInclude(ExpressionFactory<TestEntity>.Create(x => x.Child)),
                CreateInclude(ExpressionFactory<TestEntity>.Create(x => x.ChildrenCollection))
            };

            visitor.VisitIncludeExpressions(includes);

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void IncludeWithNode_ClassWithClass()
        {
            var expected = @"
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
					}
                }
            ".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Empty();
            var include = CreateInclude(ExpressionFactory<TestEntity>.Create(x => x.Child));
            include.Nodes.Add(CreateInclude(ExpressionFactory<Child>.Create(x => x.SubChild)));

            visitor.VisitIncludeExpressions(new() { include });

            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
