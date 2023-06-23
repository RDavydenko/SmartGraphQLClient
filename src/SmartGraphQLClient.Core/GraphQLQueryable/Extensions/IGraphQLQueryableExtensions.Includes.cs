using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.GraphQLQueryable.Internal;
using SmartGraphQLClient.Core.GraphQLQueryable.Utils;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartGraphQLClient
{
    public static partial class IGraphQLQueryableExtensions
    {
        public static IIncludableGraphQLQueryable<TEntity, TProperty> Include<TEntity, TProperty>(
            this IGraphQLQueryable<TEntity> source,
            Expression<Func<TEntity, TProperty>> expression)
        {
            return (IIncludableGraphQLQueryable<TEntity, TProperty>)
                GraphQLQueryableMethods.GetIncludeMethod(typeof(TEntity), typeof(TProperty))
                    .Invoke(source, new object[] { expression })!;
        }

        public static IIncludableGraphQLQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableGraphQLQueryable<TEntity, IEnumerable<TPreviousProperty>> source,
            Expression<Func<TPreviousProperty, TProperty>> expression)
        {
            // Conversion from <TEntity, IEnumerable<TPreviousProperty>> to <TEntity, TPreviousProperty>
            var previousPropertyType = source.GetType().GetGenericArguments()[1];
            var convertedSource = typeof(IncludableGraphQLQueryableHelper)
                .GetMethod("ConvertTo", BindingFlags.Static | BindingFlags.Public)!
                .MakeGenericMethod(typeof(TEntity), previousPropertyType, typeof(TPreviousProperty))
                .Invoke(null, new object[] { source });

            return (IIncludableGraphQLQueryable<TEntity, TProperty>)
                GraphQLQueryableMethods.GetThenIncludeMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty))
                    .Invoke(convertedSource, new object[] { expression })!;
        }

        public static IIncludableGraphQLQueryable<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(
            this IIncludableGraphQLQueryable<TEntity, TPreviousProperty> source,
            Expression<Func<TPreviousProperty, TProperty>> expression)
        {
            return (IIncludableGraphQLQueryable<TEntity, TProperty>)
                GraphQLQueryableMethods.GetThenIncludeMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty))
                    .Invoke(source, new object[] { expression })!;
        }
    }
}
