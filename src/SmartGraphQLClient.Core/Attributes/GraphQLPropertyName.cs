namespace SmartGraphQLClient.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class GraphQLPropertyNameAttribute : Attribute
    {
        public GraphQLPropertyNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
