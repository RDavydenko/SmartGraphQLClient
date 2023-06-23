namespace SmartGraphQLClient.Core.Extensions
{
    internal static class IEnumerableExpresions
    {
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> expression, bool condition)
        {
            if (condition)
            {
                return source.Where(expression);
            }

            return source;
        }
    }
}
