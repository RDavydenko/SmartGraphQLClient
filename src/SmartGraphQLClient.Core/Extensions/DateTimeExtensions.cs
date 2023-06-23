namespace SmartGraphQLClient.Core.Extensions
{
    internal static class DateTimeExtensions
    {
        public static string ToUniversalIso8601(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("u").Replace(" ", "T");
        }

        public static string? ToUniversalIso8601(this DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToUniversalIso8601() : null;
        }
    }
}
