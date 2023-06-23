using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    public partial class SelectExpressionVisitorTests
    {
        [TestMethod]
        public void Select_AnonymousObject_WithTernaryOperator()
        {
            var expected = @"
                id 
                name 
                child { 
                    age 
                    type 
                    summary 
                    array 
                    isStarted 
                    isCompleted 
                }".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    ChildAge =
                        x.Child != null
                            ? x.Child.Age
                            : (int?)null
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithTernaryOperator_Revesed()
        {
            var expected = @"
                id 
                name 
                child { 
                    age 
                    type 
                    summary 
                    array 
                    isStarted 
                    isCompleted 
                }".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    ChildAge =
                        x.Child == null
                            ? (int?)null
                            : x.Child.Age
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithTernaryOperator_NestedTernaries()
        {
            var expected = @"
                id 
                name 
                child { 
                    subChild { 
                        type 
                        age 
                        summary 
                        array 
                        isStarted 
                        isCompleted 
                    } 
                    age 
                    type
                    summary 
                    array 
                    isStarted 
                    isCompleted 
                }".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    ChildSubChildType =
                        x.Child != null
                            ? x.Child.SubChild != null
                                ? x.Child.SubChild.Type
                                : null
                            : null
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithTernaryOperator_LogicalOperation()
        {
            var expected = @"
                id 
                name 
                child { 
                    subChild { 
                        subChild { 
                            isStarted 
                            age 
                            type 
                            summary 
                            array 
                            isCompleted 
                        } 
                        age 
                        type 
                        summary 
                        array 
                        isStarted 
                        isCompleted 
                    } 
                    age 
                    type 
                    summary 
                    array 
                    isStarted 
                    isCompleted 
                }".Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    ChildSubChildSubChildIsStarted =
                        x.Child != null && x.Child.SubChild != null && x.Child.SubChild.SubChild != null
                            ? x.Child.SubChild.SubChild.IsStarted
                            : (bool?)null
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
