using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.GraphQLQueryable.Utils;
using SmartGraphQLClient.Core.Models.Constants;

namespace SmartGraphQLClient.Extensions
{
    public static partial class IGraphQLQueryableInternalExtensions
    {
        public static IGraphQLQueryable<TEntity> DisableIgnoreAttributes<TEntity>(
            this IGraphQLQueryable<TEntity> source)
        {
            return (IGraphQLQueryable<TEntity>)
                GraphQLQueryableMethods.GetConfigureMethod(typeof(TEntity))
                    .Invoke(source, new object?[] { QueryableConfigurationKeys.DisabledIgnoreAttributes, true })!;
        }
    }
}
