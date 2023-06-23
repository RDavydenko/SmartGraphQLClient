using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Core.GraphQLQueryable;
using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Tests.TestsInfrastructure;

namespace SmartGraphQLClient.Tests.Core.GraphQLQueryable
{
    [TestClass]
    public partial class GraphQLQueryableTests : TestBase
    {
        private IGraphQLQueryable<T> CreateQueryable<T>()
            => new GraphQLQueryable<T>(null!, ServiceProvider, null!);
    }
}
