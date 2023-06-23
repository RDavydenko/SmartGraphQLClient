using SmartGraphQLClient.Core.Providers.Abstractions;

namespace SmartGraphQLClient.Core.Providers
{
    internal class DefaultGraphQLOffsetPaginationStringProvider : IGraphQLOffsetPaginationStringProvider
    {
        public string? BuildSkip(int? skip)
        {
            const string keyword = "skip";

            if (skip is null) return null;

            return $"{keyword}: {skip}";
        }

        public string? BuildTake(int? take)
        {
            const string keyword = "take";

            if (take is null) return null;

            return $"{keyword}: {take}";
        }
    }
}
