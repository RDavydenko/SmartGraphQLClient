using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartGraphQLClient.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.Core.GraphQLHttpClient
{
    public partial class GraphQLHttpClientTests
    {
        [TestMethod]
        public async Task RemotePipeline_And_LocalPipeline()
        {
            using var client = CreateClient();

            var page = await client.Query<UserModel>("usersPage")
                .Where(x => x.Id > 1)                               // query
                .Where(x => x.Id > 2)                               // query
                .Take(2)                                            // query
                .Select(x => new { x.Id })                          // query
                .Where(x => x.Id > 3)                               // local
                .Select(x => new { x.Id, DoubledId = x.Id * 2 })    // local
                .Skip(0)                                            // local
                .Take(200)                                          // local
                .ToPageAsync();                                     // executing query + executing local expressions

            Assert.IsNotNull(page);
            Assert.IsTrue(page.Items.Any());
            Assert.IsTrue(page.Items.All(x => x.Id > 3));   // by local filter
            Assert.IsTrue(page.Items.Count() <= 2);         // by query take
        }


        [TestMethod]
        public async Task RemotePipeline_And_LocalPipeline_V2()
        {
            using var client = CreateClient();

            var items = await client.Query<UserModel>("users")
                .Where(x => x.Id > 1)                               // query
                .Where(x => x.Id > 2)                               // query
                // .Take(2)                                         // not supported (has no [UseOffsetPaging])
                .Select(x => new { x.Id })                          // query
                .Where(x => x.Id > 3)                               // local
                .Select(x => new { x.Id, DoubledId = x.Id * 2 })    // local
                .Skip(0)                                            // local (supported - local)
                .Take(200)                                          // local (supported - local)
                .ToArrayAsync();                                    // executing query + executing local expressions

            Assert.IsNotNull(items);
            Assert.IsTrue(items.Any());
            Assert.IsTrue(items.All(x => x.Id > 3));    // by local filter
            Assert.IsTrue(items.Count() <= 200);        // by local take
        }
    }
}
