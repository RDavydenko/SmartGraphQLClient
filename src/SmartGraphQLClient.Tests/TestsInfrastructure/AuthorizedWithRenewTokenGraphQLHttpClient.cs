using System;
using System.Net.Http;

namespace SmartGraphQLClient.Tests.TestsInfrastructure
{
    public class AuthorizedWithRenewTokenGraphQLHttpClient : GraphQLHttpClient
    {
        public AuthorizedWithRenewTokenGraphQLHttpClient(
            HttpClient httpClient,
            IServiceProvider serviceProvider)
            : base(httpClient, serviceProvider)
        {
        }
    }
}
