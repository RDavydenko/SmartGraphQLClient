namespace SmartGraphQLClient.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class GraphQLEndpointAttribute : Attribute
    {
        public GraphQLEndpointAttribute(string endpoint)
        {
            Endpoint = endpoint;
        }

        public string Endpoint { get; }
    }
}
