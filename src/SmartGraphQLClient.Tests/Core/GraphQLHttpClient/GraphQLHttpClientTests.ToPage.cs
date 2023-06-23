using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task ToPageAsync()
        {
            using var client = CreateClient();

            var page = await client.Query<UserModel>("usersPage")
                .ToPageAsync();

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Items);
            Assert.IsNotNull(page.PageInfo);
            Assert.IsTrue(page.TotalCount > 0);
            Assert.IsTrue(page.Items.Any());
            Assert.IsTrue(page.Items.All(x => x.Id != 0));
            Assert.IsTrue(page.Items.All(x => x.UserName is not null));
        }

        [TestMethod]
        public async Task ToPageAsync_Select()
        {
            using var client = CreateClient();

            var page = await client.Query<UserModel>("usersPage")
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .ToPageAsync();

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Items);
            Assert.IsNotNull(page.PageInfo);
            Assert.IsTrue(page.TotalCount > 0);
            Assert.IsTrue(page.Items.Any());
            Assert.IsTrue(page.Items.All(x => x.EntityId != 0));
            Assert.IsTrue(page.Items.All(x => x.EntityName is not null));
        }

        [TestMethod]
        public async Task ToPageAsync_Where_Select()
        {
            using var client = CreateClient();

            var page = await client.Query<UserModel>("usersPage")
                .Where(x => x.Id > 2 && x.Roles.Any(r => r.Code == RoleCode.ADMINISTRATOR))
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .ToPageAsync();

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Items);
            Assert.IsNotNull(page.PageInfo);
            Assert.IsTrue(page.TotalCount > 0);
            Assert.IsTrue(page.Items.Any());
            Assert.IsTrue(page.Items.All(x => x.EntityId != 0));
            Assert.IsTrue(page.Items.All(x => x.EntityName is not null));
        }

        [TestMethod]
        public async Task ToPageAsync_Where_Select_Take()
        {
            using var client = CreateClient();

            var page = await client.Query<UserModel>("usersPage")
                .Where(x => x.Id > 2 && x.Roles.Any(r => r.Code == RoleCode.ADMINISTRATOR))
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .Take(3)
                .ToPageAsync();

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Items);
            Assert.IsNotNull(page.PageInfo);
            Assert.IsTrue(page.Items.Count <= 3);
            Assert.IsTrue(page.TotalCount > 0);
            Assert.IsTrue(page.Items.Any());
            Assert.IsTrue(page.Items.All(x => x.EntityId != 0));
            Assert.IsTrue(page.Items.All(x => x.EntityName is not null));
        }

        [TestMethod]
        public async Task ToPageAsync_Where_Select_Skip_Take()
        {
            using var client = CreateClient();

            var page = await client.Query<UserModel>("usersPage")
                .Where(x => x.Id > 2 && x.Roles.Any(r => r.Code == RoleCode.ADMINISTRATOR))
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .Skip(1)
                .Take(3)
                .ToPageAsync();

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Items);
            Assert.IsNotNull(page.PageInfo);
            Assert.IsTrue(page.PageInfo.HasPreviousPage);
            Assert.IsTrue(page.Items.Count <= 3);
            Assert.IsTrue(page.TotalCount > 0);
            Assert.IsTrue(page.Items.Any());
            Assert.IsTrue(page.Items.All(x => x.EntityId != 0));
            Assert.IsTrue(page.Items.All(x => x.EntityName is not null));
        }

        [TestMethod]
        public async Task ToPageAsync_Where_OrderBy_Select_Skip_Take()
        {
            using var client = CreateClient();

            var page = await client.Query<UserModel>("usersPage")
                .Where(x => x.Id > 2 && x.Roles.Any(r => r.Code == RoleCode.ADMINISTRATOR))
                .OrderByDescending(x => x.Position.Id)
                    .ThenByDescending(x => x.Id)
                .Select(x => new
                {
                    EntityId = x.Id,
                    EntityName = x.UserName
                })
                .Skip(1)
                .Take(3)
                .ToPageAsync();

            Assert.IsNotNull(page);
            Assert.IsNotNull(page.Items);
            Assert.IsNotNull(page.PageInfo);
            Assert.IsTrue(page.PageInfo.HasPreviousPage);
            Assert.IsTrue(page.Items.Count <= 3);
            Assert.IsTrue(page.TotalCount > 0);
            Assert.IsTrue(page.Items.Any());
            Assert.IsTrue(page.Items.All(x => x.EntityId != 0));
            Assert.IsTrue(page.Items.All(x => x.EntityName is not null));
        }
    }
}
