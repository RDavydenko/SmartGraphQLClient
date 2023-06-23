using SmartGraphQLClient.Core.Models.Constants;
using SmartGraphQLClient.Errors;

namespace SmartGraphQLClient.Core.Extensions
{
    internal static class GraphQLErrorsExtensions
    {
        public static bool HasAuthorizationError(this IEnumerable<GraphQLError> errors)
            => errors.Any(
                e => e.Extensions is not null &&
                     e.Extensions.TryGetValue("code", out var code) &&
                     code == GraphQLErrorConstants.AUTH_NOT_AUTHORIZED);
    }
}
