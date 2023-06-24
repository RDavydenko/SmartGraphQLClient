using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartGraphQLClient.Core.Services.Abstractions;
using SmartGraphQLClient.GraphQLServer;
using SmartGraphQLClient.Tests.TestsInfrastructure.Services;

namespace SmartGraphQLClient.Tests.TestsInfrastructure;

public class IntegrationTestBase : TestBase
{
    const int Port = 7023;
    const string TestApplicationUrlKey = "TestApplicationUrl";
    private static readonly string TestApplicationUrl = $"https://localhost:{Port}/";

    protected override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);

        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => { builder.UseSetting("https_port", Port.ToString()); });

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [TestApplicationUrlKey] = TestApplicationUrl
            })
            .Build();

        services.AddScoped<IConfiguration>((_) => configuration);
        services.AddMemoryCache();

        // simple not authorized client
        AddIntegrationTestGraphQLHttpClient(services, factory); // integration test
        // AddTestGraphQLHttpClient(services); // real usage example

       
        // authorized-client with token provider and cache token (optional)
        services
            .AddScoped<IGraphQLAuthorizationService<AuthorizedTestGraphQLHttpClient>,
                TestAuthorizationService>();
        
        AddIntegrationTestAuthorizedGraphQLHttpClient(services, factory); // integration test
        // AddTestAuthorizedGraphQLHttpClient(services); // real usage example
    }

    // For production usage (example)
    private static void AddTestAuthorizedGraphQLHttpClient(IServiceCollection services)
    {
        services.AddHttpClient<AuthorizedTestGraphQLHttpClient>(
            nameof(AuthorizedTestGraphQLHttpClient),
            (sp, client) =>
            {
                var cachedToken = sp.GetRequiredService<IMemoryCache>()
                    .Get<string>($"BearerToken_{nameof(AuthorizedTestGraphQLHttpClient)}");
                client.BaseAddress = new Uri(TestApplicationUrl + "graphql/");
                if (cachedToken is not null)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cachedToken);
                }
            });
    }

    // For production usage (example)
    private static void AddTestGraphQLHttpClient(IServiceCollection services)
    {
        services.AddHttpClient<TestGraphQLHttpClient>(
            nameof(TestGraphQLHttpClient),
            client => { client.BaseAddress = new Uri(TestApplicationUrl + "graphql/"); });
    }


    // For integration tests (not for production usage)
    private static void AddIntegrationTestGraphQLHttpClient(IServiceCollection services, WebApplicationFactory<Program> factory)
    {
        services.AddScoped<TestGraphQLHttpClient>(sp =>
        {
            var client = factory.CreateClient();
            client.BaseAddress = new Uri(TestApplicationUrl + "graphql/");
            return new TestGraphQLHttpClient(client, sp);
        });
    }
    
    // For integration tests (not for production usage)
    private static void AddIntegrationTestAuthorizedGraphQLHttpClient(IServiceCollection services, WebApplicationFactory<Program> factory)
    {
        services.AddScoped<AuthorizedTestGraphQLHttpClient>(
            sp =>
            {
                var client = factory.CreateClient();
                var cachedToken = sp.GetRequiredService<IMemoryCache>()
                    .Get<string>($"BearerToken_{nameof(AuthorizedTestGraphQLHttpClient)}");
                client.BaseAddress = new Uri(TestApplicationUrl + "graphql/");
                if (cachedToken is not null)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", cachedToken);
                }

                return new AuthorizedTestGraphQLHttpClient(client, sp);
            });
    }
}