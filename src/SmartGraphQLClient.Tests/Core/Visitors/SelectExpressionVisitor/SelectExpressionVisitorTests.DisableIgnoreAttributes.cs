using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities;
using SmartGraphQLClient.Tests.TestsInfrastructure.Extensions;

namespace SmartGraphQLClient.Tests.Core.Visitors.SelectExpressionVisitor
{
    public partial class SelectExpressionVisitorTests
    {
        [TestMethod]
        public void Select_AnonymousObject_DisabledIgnoreAttributes()
        {
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
                anotherSimpleProperty
                ".Tokenize();
            var visitor = VisitorFactory<TestEntity>(new SelectExpressionVisitorConfiguration() { DisabledIgnoreAttributes = true })
                .Select((x) => new { Entity = x });

            visitor.Visit();
            var tokens = visitor.ToString().Tokenize();

            CollectionAssert.AreEqual(expected, tokens);
        }
    }
}
