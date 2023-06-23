using System.Text.Json.Serialization;

namespace SmartGraphQLClient
{
    public class CollectionSegment<T>
    {
        public CollectionSegment()
        {
        }

        protected CollectionSegment(
            T[] items,
            CollectionSegmentInfo pageInfo,
            int totalCount)
        {
            Items = items;
            PageInfo = pageInfo;
            TotalCount = totalCount;
        }

        [JsonInclude] public IReadOnlyCollection<T> Items { get; private set; } = Array.Empty<T>();
        [JsonInclude] public CollectionSegmentInfo PageInfo { get; private set; } = CollectionSegmentInfo.Empty;
        [JsonInclude] public int TotalCount { get; private set; }
    }
}
