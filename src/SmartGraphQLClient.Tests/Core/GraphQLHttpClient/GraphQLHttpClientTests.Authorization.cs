using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using SmartGraphQLClient.Core.Models.Constants;
using SmartGraphQLClient.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task AuthorizedQuery_WithoutToken_ShouldThrowException()
        {
            using var client = CreateClient();

            var task = client.Query<UserModel>("admins")
                .ToListAsync();

            await Assert.ThrowsExceptionAsync<GraphQLException>(() => task);
        }

        [TestMethod]
        public async Task AuthorizedQuery_WithoutToken_ShouldThrowExceptionWithCode()
        {
            using var client = CreateClient();

            try
            {
                var task = await client.Query<UserModel>("admins")
                    .ToListAsync();
            }
            catch (GraphQLException ex)
            {
                Assert.IsNotNull(ex.Errors);
                Assert.IsNotNull(ex.Message);
                Assert.IsTrue(ex.Errors.Any());
                Assert.IsTrue(ex.Errors[0].Extensions.ContainsKey("code"));
                Assert.AreEqual(GraphQLErrorConstants.AUTH_NOT_AUTHORIZED, ex.Errors[0].Extensions["code"]);

                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public async Task AuthorizedQuery_WithToken_ShouldBeSuccess()
        {
            using var client = CreateAuthorizedClient();

            var admins = await client.Query<UserModel>("admins")
                .ToListAsync();

            Assert.IsNotNull(admins);
            Assert.IsTrue(admins.Any());
            Assert.IsTrue(admins.All(x => x.Id != 0));
            Assert.IsTrue(admins.All(x => x.UserName is not null));
        }

        [TestMethod]
        public async Task AuthorizedQuery_WithAuthorizationService_ShouldBeSuccess()
        {
            using var client = CreateAuthorizedClient();

            var admins = await client.Query<UserModel>("admins")
                .ToListAsync();

            Assert.IsNotNull(admins);
            Assert.IsTrue(admins.Any());
            Assert.IsTrue(admins.All(x => x.Id != 0));
            Assert.IsTrue(admins.All(x => x.UserName is not null));

            // the query should use configured token
            admins = await client.Query<UserModel>("admins")
                .ToListAsync();

            Assert.IsNotNull(admins);
            Assert.IsTrue(admins.Any());
            Assert.IsTrue(admins.All(x => x.Id != 0));
            Assert.IsTrue(admins.All(x => x.UserName is not null));

            using var client2 = CreateAuthorizedClient();

            // the query should use cached token
            admins = await client2.Query<UserModel>("admins")
                .ToListAsync();

            Assert.IsNotNull(admins);
            Assert.IsTrue(admins.Any());
            Assert.IsTrue(admins.All(x => x.Id != 0));
            Assert.IsTrue(admins.All(x => x.UserName is not null));
        }
    }
}
