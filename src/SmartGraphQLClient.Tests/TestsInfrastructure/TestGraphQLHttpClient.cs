using System;
using System.Net.Http;

namespace SmartGraphQLClient.Tests.TestsInfrastructure
{
    public class TestGraphQLHttpClient : GraphQLHttpClient
    {
        public TestGraphQLHttpClient(
            HttpClient httpClient,
            IServiceProvider serviceProvider)
            : base(httpClient, serviceProvider)
        {
        }
    }
}
