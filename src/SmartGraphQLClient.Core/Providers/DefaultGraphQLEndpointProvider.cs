using SmartGraphQLClient.Attributes;
using SmartGraphQLClient.Core.Providers.Abstractions;
using System.Reflection;

namespace SmartGraphQLClient.Core.Providers
{
    internal class DefaultGraphQLEndpointProvider : IGraphQLEndpointProvider
    {
        public string GetGraphQLEndpoint(Type entityType)
        {
            var endpointAttribute = entityType.GetCustomAttribute<GraphQLEndpointAttribute>(inherit: true);
            if (endpointAttribute is null) throw new InvalidOperationException();

            return endpointAttribute.Endpoint;
        }
    }
}
