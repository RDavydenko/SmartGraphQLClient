namespace SmartGraphQLClient.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GraphQLIgnoreAttribute : Attribute
    {
        public GraphQLIgnoreAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }

        public bool Ignore { get; }
    }
}
