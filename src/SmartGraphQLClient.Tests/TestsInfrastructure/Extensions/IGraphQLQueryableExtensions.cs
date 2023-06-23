namespace SmartGraphQLClient.Tests.TestsInfrastructure.Extensions
{
    internal static class IGraphQLQueryableExtensions
    {
        public static string GetDebugView(this object queryable)
        {
            return (queryable as dynamic).DebugView;
        }
    }
}
