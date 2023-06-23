namespace SmartGraphQLClient
{
    public class GraphQLRequestConfiguration
    {
        public string? Endpoint { get; set; }

        internal bool IsArray { get; set; }
        internal bool IsCollectionPage { get; set; }
        internal bool IsFirstOrDefault { get; set; }

        internal void ClearInternal()
        {
            IsArray = false;
            IsCollectionPage = false;
            IsFirstOrDefault = false;
        }
    }
}
