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

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSmartGraphQLClient();
        }
    }
}
