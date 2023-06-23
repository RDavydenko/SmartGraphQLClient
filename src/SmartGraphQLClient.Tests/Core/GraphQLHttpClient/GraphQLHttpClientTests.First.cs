using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using SmartGraphQLClient.Exceptions;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task FirstAsync()
        {
            using var client = CreateClient();

            var entity = await client.Query<UserModel>("user")
                .FirstAsync();

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id != 0);
            Assert.IsTrue(entity.UserName is not null);
        }

        [TestMethod]
        public async Task FirstAsync_Select()
        {
            using var client = CreateClient();

            var entity = await client.Query<UserModel>("user")
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .FirstAsync();

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.EntityId != 0);
            Assert.IsTrue(entity.EntityName is not null);
        }

        [TestMethod]
        public async Task FirstAsync_Argument()
        {
            using var client = CreateClient();

            var entity = await client.Query<UserModel>("user")
                .Argument("id", 4)
                .FirstAsync();

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id == 4);
            Assert.IsTrue(entity.UserName is not null);
        }

        [TestMethod]
        public async Task FirstAsync_Argument_Select()
        {
            using var client = CreateClient();

            var entity = await client.Query<UserModel>("user")
                .Argument("id", 4)
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .FirstAsync();

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.EntityId == 4);
            Assert.IsTrue(entity.EntityName is not null);
        }

        [TestMethod]
        public async Task FirstAsync_ServerException_ShouldThrowGraphQLException()
        {
            using var client = CreateClient();
            var firstAsyncTask = client.Query<UserModel>("user")
                .Argument("id", -1)
                .FirstAsync();

            await Assert.ThrowsExceptionAsync<GraphQLException>(() => firstAsyncTask);
        }
    }
}
