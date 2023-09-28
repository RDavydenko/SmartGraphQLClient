using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task ExecuteRawQuery_UserById_ShouldBeSuccess()
        {
            using var client = CreateClient();

            var user = await client.ExecuteRawQueryAsync<UserModel>(@"
                {
                    user(id: 1) {
                        id
                        userName
                        age
                    }
                }
            ");

            Assert.IsNotNull(user);
            Assert.IsTrue(user.Id == 1);
            Assert.IsTrue(!string.IsNullOrEmpty(user.UserName));
            Assert.IsTrue(user.Age != default);
        }
        
        [TestMethod]
        public async Task ExecuteRawQuery_UserByIdWithCollection_ShouldBeSuccess()
        {
            using var client = CreateClient();

            var user = await client.ExecuteRawQueryAsync<UserModel>(@"
                {
                    user(id: 1) {
                        id
                        userName
                        age
                        roles {
                            id
                            code
                        }
                    }
                }
            ");

            Assert.IsNotNull(user);
            Assert.IsTrue(user.Id == 1);
            Assert.IsTrue(!string.IsNullOrEmpty(user.UserName));
            Assert.IsTrue(user.Age != default);
            Assert.IsNotNull(user.Roles);
            Assert.IsTrue(user.Roles.Any());
            Assert.IsTrue(user.Roles.All(r => r.Id != default));
        }
        
        [TestMethod]
        public async Task ExecuteRawQuery_UsersList_ShouldBeSuccess()
        {
            using var client = CreateClient();

            var users = await client.ExecuteRawQueryAsync<List<UserModel>>(@"
                {
                    users(
                        where: { id: { gt: 1 } }
                        order: [{ id: DESC }]
                    ) {
                        id
                        userName
                        age
                    }
                }
            ");

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());
            Assert.IsTrue(users.All(x => x.Id > 1));
            Assert.IsTrue(users.All(x => x.Id != default));
            Assert.IsTrue(users.All(x => x.Age != default));
            Assert.IsTrue(users.All(x => !string.IsNullOrEmpty(x.UserName)));
            CollectionAssert.AreEqual(users.OrderByDescending(x => x.Id).ToList(), users);
        }
        
        [TestMethod]
        public async Task ExecuteRawQuery_UsersArray_ShouldBeSuccess()
        {
            using var client = CreateClient();

            var users = await client.ExecuteRawQueryAsync<UserModel[]>(@"
                {
                    users(
                        where: { id: { gt: 1 } }
                        order: [{ id: DESC }]
                    ) {
                        id
                        userName
                        age
                    }
                }
            ");

            Assert.IsNotNull(users);
            Assert.IsTrue(users.Any());
            Assert.IsTrue(users.All(x => x.Id > 1));
            Assert.IsTrue(users.All(x => x.Id != default));
            Assert.IsTrue(users.All(x => x.Age != default));
            Assert.IsTrue(users.All(x => !string.IsNullOrEmpty(x.UserName)));
            CollectionAssert.AreEqual(users.OrderByDescending(x => x.Id).ToArray(), users);
        }
        
        [TestMethod]
        public async Task ExecuteRawQuery_UsersPage_ShouldBeSuccess()
        {
            using var client = CreateClient();

            var usersPage = await client.ExecuteRawQueryAsync<CollectionSegment<UserModel>>(@"
                {
                    usersPage(
                        where: { id: { gt: 1 } }
                        order: [{ id: DESC }]
                        skip: 1
                        take: 2
                    ) {
                        items {
                            id
                            userName
                            age
                        }
                        pageInfo { hasNextPage hasPreviousPage }
                        totalCount
                    }
                }
            ");

            Assert.IsNotNull(usersPage);
            Assert.IsNotNull(usersPage.PageInfo);
            Assert.IsTrue(usersPage.TotalCount != 0);
            Assert.IsTrue(usersPage.Items.Count <= 2);
            Assert.IsTrue(usersPage.Items.All(x => x.Id > 1));
            Assert.IsTrue(usersPage.Items.All(x => x.Id != default));
            Assert.IsTrue(usersPage.Items.All(x => x.Age != default));
            Assert.IsTrue(usersPage.Items.All(x => !string.IsNullOrEmpty(x.UserName)));
            CollectionAssert.AreEqual(usersPage.Items.OrderByDescending(x => x.Id).ToList(), usersPage.Items.ToList());
        }
        
        [TestMethod]
        public async Task ExecuteRawQuery_Scalar_ShouldBeSuccess()
        {
            using var client = CreateClient();

            var integer = await client.ExecuteRawQueryAsync<int>(@"
                {
                    integer(id: 1)
                }
            ");

            Assert.IsTrue(integer == 1);
        }
    }
}
