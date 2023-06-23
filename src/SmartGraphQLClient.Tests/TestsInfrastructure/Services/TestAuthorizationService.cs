using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SmartGraphQLClient.Core.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartGraphQLClient.Tests.TestsInfrastructure.Services
{
    internal class TestAuthorizationService : IGraphQLAuthorizationService<AuthorizedWithRenewTokenGraphQLHttpClient>
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public TestAuthorizationService(
            IConfiguration configuration,
            IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        public async Task Authorize(HttpClient httpClient, CancellationToken cancellationToken)
        {
            var testApplicationUrl = _configuration["TestApplicationUrl"];
            using var response = await httpClient.PostAsync(new Uri(testApplicationUrl + "auth/token", UriKind.Absolute), null, cancellationToken);
            var token = await response.Content.ReadAsStringAsync(cancellationToken);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            _memoryCache.Set($"BearerToken_{nameof(AuthorizedTestGraphQLHttpClient)}", token);
        }
    }
}
