using System;
using System.Net.Http;

namespace SmartGraphQLClient.Tests.TestsInfrastructure
{
    public class AuthorizedTestGraphQLHttpClient : GraphQLHttpClient
    {
        public AuthorizedTestGraphQLHttpClient(
            HttpClient httpClient,
            IServiceProvider serviceProvider)
            : base(httpClient, serviceProvider)
        {
        }
    }
}
