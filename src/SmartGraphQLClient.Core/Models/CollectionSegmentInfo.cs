using System.Text.Json.Serialization;

namespace SmartGraphQLClient
{
    public class CollectionSegmentInfo
    {
        internal static CollectionSegmentInfo Empty => new();

        [JsonInclude] public bool HasNextPage { get; private set; }
        [JsonInclude] public bool HasPreviousPage { get; private set; }
    }
}
