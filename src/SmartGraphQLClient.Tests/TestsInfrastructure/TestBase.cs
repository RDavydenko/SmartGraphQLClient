using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using SmartGraphQLClient.Core;
using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Services.Abstractions;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using SmartGraphQLClient.Tests.TestsInfrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace SmartGraphQLClient.Tests.TestsInfrastructure
{
    public class TestBase
    {
        private readonly IServiceProvider _serviceProvider;

        protected TestBase()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected IServiceProvider ServiceProvider => _serviceProvider.CreateScope().ServiceProvider;

        protected Factories.VisitorFactory<T> VisitorFactory<T>(
            SelectExpressionVisitorConfiguration? configuration = null)
            => new Factories.VisitorFactory<T>(_serviceProvider, configuration);

        protected IncludeExpressionNode CreateInclude(LambdaExpression expression)
            => new IncludeExpressionNode(expression);

        private static void ConfigureServices(IServiceCollection services)
        {
            const string TestApplicationUrlKey = "TestApplicationUrl";
            const string TestApplicationUrl = "https://localhost:7023/";

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    [TestApplicationUrlKey] = TestApplicationUrl
                })
                .Build();

            services.AddScoped<IConfiguration>((_) => configuration);
            services.AddMemoryCache();
            services.AddSmartGraphQLClient();

            // simple not authorized client
            services.AddHttpClient<TestGraphQLHttpClient>(
                nameof(TestGraphQLHttpClient),
                client =>
                {
                    client.BaseAddress = new Uri(TestApplicationUrl + "graphql/");
                });

            // client with token
            try
            {
                var token = new HttpClient().PostAsync(new Uri(TestApplicationUrl + "auth/token", UriKind.Absolute), null).Result.Content.ReadAsStringAsync().Result;
                services.AddHttpClient<AuthorizedTestGraphQLHttpClient>(
                    nameof(AuthorizedTestGraphQLHttpClient),
                    client =>
                    {
                        client.BaseAddress = new Uri(TestApplicationUrl + "graphql/");
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    });
            }
            catch { }

            // client with token provider with cache
            services.AddScoped<IGraphQLAuthorizationService<AuthorizedWithRenewTokenGraphQLHttpClient>, TestAuthorizationService>();
            services.AddHttpClient<AuthorizedWithRenewTokenGraphQLHttpClient>(
                nameof(AuthorizedWithRenewTokenGraphQLHttpClient),
                (sp, client) =>
                {
                    var cachedToken = sp.GetRequiredService<IMemoryCache>().Get<string>($"BearerToken_{nameof(AuthorizedTestGraphQLHttpClient)}");
                    client.BaseAddress = new Uri(TestApplicationUrl + "graphql/");
                    if (cachedToken is not null)
                    {
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cachedToken);
                    }
                });
        }
    }
}
