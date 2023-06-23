using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    public partial class SelectExpressionVisitorTests
    {
        [TestMethod]
        public void Select_AnonymousObject_WithSubClass()
        {
            var expected = @"
                id 
                name 
                updateDate 
                child { 
                    age 
                    type 
                    summary 
                    array 
                    isStarted 
                    isCompleted 
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    x.UpdateDate,
                    x.Child
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithSubClass_WithSubClass()
        {
            var expected = @"
                id 
                name 
                updateDate 
                child { 
                    subChild { 
                        age 
                        type 
                        summary 
                        array 
                        isStarted 
                        isCompleted 
                    } 
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    x.UpdateDate,
                    x.Child.SubChild
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithSubClass_WithOneFieldSubClass()
        {
            var expected = @"
                id 
                name 
                updateDate 
                child { 
                    subChild { 
                        age
                    } 
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    x.UpdateDate,
                    x.Child.SubChild.Age
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithSubClass_TwoFieldsToAnonymousObject()
        {
            var expected = @"
                id 
                name
                child { 
                    isStarted
                    isCompleted
                }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                x => new
                {
                    x.Id,
                    x.Name,
                    ChildInfo = new
                    {
                        x.Child.IsStarted,
                        x.Child.IsCompleted,
                    }
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }

        [TestMethod]
        public void Select_AnonymousObject_WithSubClassAndSomeFields_WithSubClass()
        {
            var expected = @"
                    id 
                    name 
                    updateDate 
                    child { 
                        subChild { 
                            age 
                            type 
                            summary 
                            array 
                            isStarted 
                            isCompleted 
                        } 
                        isStarted 
                    }"
                .Tokenize();
            var visitor = VisitorFactory<TestEntity>().Select(
                (x) => new
                {
                    x.Id,
                    x.Name,
                    x.UpdateDate,
                    x.Child.SubChild,
                    ChildIsStarted = x.Child.IsStarted,
                    ChildSubChildIsStarted = x.Child.SubChild.IsStarted
                }
            );

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
