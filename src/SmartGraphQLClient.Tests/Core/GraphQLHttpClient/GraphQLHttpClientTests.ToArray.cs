using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task ToArrayAsync()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .ToArrayAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.Id != 0));
            Assert.IsTrue(items.All(x => x.UserName is not null));
        }

        [TestMethod]
        public async Task ToArrayAsync_Select()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .ToArrayAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.EntityId != 0));
            Assert.IsTrue(items.All(x => x.EntityName is not null));
        }

        [TestMethod]
        public async Task ToArrayAsync_Where_Select_Select_Select_Select()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .Where(x => x.Id >= 2)
                .Select(x => new
                {
                    Entity = x,
                    EntityId = x.Id,
                    EntityName = x.UserName,
                })
                .Select(x => x.Entity)
                .Select(x => new { UniqieId = x.Id })
                .Select(x => x.UniqieId)
                .ToArrayAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(id => id >= 2));
        }

        [TestMethod]
        public async Task ToArrayAsync_WithoutInclude()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .ToArrayAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.Id != 0));
            Assert.IsTrue(items.All(x => x.UserName is not null));
            Assert.IsTrue(items.All(x => x.Roles?.Any() != true));
        }

        [TestMethod]
        public async Task ToArrayAsync_WithInclude()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .Include(x => x.Roles)
                .ToArrayAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.Id != 0));
            Assert.IsTrue(items.All(x => x.UserName is not null));
            Assert.IsTrue(items.All(x => x.Roles is not null));
            Assert.IsTrue(items.Any(x => x.Roles.Any()));
        }

        [TestMethod]
        public async Task ToArrayAsync_WithLongIncludesChain()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .Where(x => x.Position.Id > 0) // Position != null
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Users)
                .Include(x => x.Position)
                    .ThenInclude(x => x.Users)
                        .ThenInclude(x => x.Roles)
                .ToArrayAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.Id != 0));
            Assert.IsTrue(items.All(x => x.UserName is not null));
            Assert.IsTrue(items.All(x => x.Roles is not null));
            Assert.IsTrue(items.All(x => x.Roles.All(xx => xx.Users is not null)));
            Assert.IsTrue(items.All(x => x.Position is not null));
            Assert.IsTrue(items.All(x => x.Position.Users is not null));
            Assert.IsTrue(items.All(x => x.Position.Users.All(xx => xx.Roles is not null)));
        }

        [TestMethod]
        public async Task ToArrayAsync_LongCallChain()
        {
            using var client = CreateClient();

            var chiefName = "Chief";

            var items = await client.Query<UserModel>("users")
                .Include(x => x.Roles)
                    .ThenInclude(x => x.Users)
                .Where(x => x.UserName.StartsWith("R") && x.Id >= 1)
                .Where(x => x.Roles.Any(m => m.Code == RoleCode.ADMINISTRATOR) || x.Position.Name == chiefName)
                .Select(x => new
                {
                    x.Id,
                    Name = x.UserName,
                    IsChief = x.Position != null && x.Position.Name == chiefName,
                    CountOtherChiefs = x.Position != null 
                        ? x.Position.Users.Length
                        : 0,
                    IsAdministrator = x.Roles.Any(x => x.Code == RoleCode.ADMINISTRATOR),
                    CountOfAdministrators = x.Roles.Any(x => x.Code == RoleCode.ADMINISTRATOR)
                        ? x.Roles.First(x => x.Code == RoleCode.ADMINISTRATOR).Users.Length
                        : 0
                })
                .ToArrayAsync();

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
        }
    }
}
