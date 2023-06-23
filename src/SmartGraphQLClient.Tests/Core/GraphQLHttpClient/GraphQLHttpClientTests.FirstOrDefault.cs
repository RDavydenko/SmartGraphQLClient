using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task FirstOrDefaultAsync()
        {
            using var client = CreateClient();

            var entity = await client.Query<UserModel>("user")
                .FirstOrDefaultAsync();

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.Id != 0);
            Assert.IsTrue(entity.UserName is not null);
        }

        [TestMethod]
        public async Task FirstOrDefaultAsync_Select()
        {
            using var client = CreateClient();

            var entity = await client.Query<UserModel>("user")
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .FirstOrDefaultAsync();

            Assert.IsNotNull(entity);
            Assert.IsTrue(entity.EntityId != 0);
            Assert.IsTrue(entity.EntityName is not null);
        }
    }
}
