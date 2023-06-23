using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor;
using SmartGraphQLClient.Tests.TestsInfrastructure;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    [TestClass]
    public class DefaultSelectExpressionVisitorTests : TestBase
    {
        [TestMethod]
        public void ShouldIncludeAllSimpleTypes()
        {
            var visitor = new DefaultSelectExpressionVisitor(typeof(TestEntity), ServiceProvider, new());
            var expected = @"
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
                ".Tokenize();

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
