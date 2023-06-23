using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task ToListAsync()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .ToListAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.Id != 0));
            Assert.IsTrue(items.All(x => x.UserName is not null));
        }

        [TestMethod]
        public async Task ToListAsync_Select()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .ToListAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.EntityId != 0));
            Assert.IsTrue(items.All(x => x.EntityName is not null));
        }

        [TestMethod]
        public async Task ToListAsync_SelectInt32()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .Select(x => x.Id)
                .ToListAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(id => id != 0));
        }
    }
}
