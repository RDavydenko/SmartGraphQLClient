using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.GraphQLQueryable.Utils;
using System.Linq.Expressions;

namespace SmartGraphQLClient
{
    public static partial class IGraphQLQueryableExtensions
    {
        public static IGraphQLQueryable<TEntity> Where<TEntity>(
            this IGraphQLQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate)
        {
            return (IGraphQLQueryable<TEntity>)
                GraphQLQueryableMethods.GetWhereMethod(typeof(TEntity))
                    .Invoke(source, new object[] { predicate })!;
        }

        public static IGraphQLQueryable<TOutEntity> Select<TEntity, TOutEntity>(
            this IGraphQLQueryable<TEntity> source, Expression<Func<TEntity, TOutEntity>> selector)
        {
            return (IGraphQLQueryable<TOutEntity>)
                GraphQLQueryableMethods.GetSelectMethod(typeof(TEntity), typeof(TOutEntity))
                    .Invoke(source, new object[] { selector })!;
        }

        public static IGraphQLQueryable<TEntity> Skip<TEntity>(
            this IGraphQLQueryable<TEntity> source, int count)
        {
            return (IGraphQLQueryable<TEntity>)
                GraphQLQueryableMethods.GetSkipMethod(typeof(TEntity))
                    .Invoke(source, new object[] { count })!;
        }

        public static IGraphQLQueryable<TEntity> Take<TEntity>(
            this IGraphQLQueryable<TEntity> source, int count)
        {
            return (IGraphQLQueryable<TEntity>)
                GraphQLQueryableMethods.GetTakeMethod(typeof(TEntity))
                    .Invoke(source, new object[] { count })!;
        }

        public static IGraphQLQueryable<TEntity> Argument<TEntity>(
            this IGraphQLQueryable<TEntity> source, string key, object? value)
        {
            return (IGraphQLQueryable<TEntity>)
                GraphQLQueryableMethods.GetArgumentMethod(typeof(TEntity))
                    .Invoke(source, new object?[] { key, value })!;
        }
    }
}
