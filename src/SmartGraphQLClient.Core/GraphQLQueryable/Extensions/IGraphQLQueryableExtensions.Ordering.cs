using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.GraphQLQueryable.Utils;
using System.Linq.Expressions;

namespace SmartGraphQLClient
{
    public static partial class IGraphQLQueryableExtensions
    {
        public static IOrderableGraphQLQueryable<TEntity, TProperty> OrderBy<TEntity, TProperty>(
            this IGraphQLQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> expression)
        {
            return (IOrderableGraphQLQueryable<TEntity, TProperty>)
                GraphQLQueryableMethods.GetOrderByMethod(typeof(TEntity), typeof(TProperty))
                    .Invoke(source, new object[] { expression })!;
        }

        public static IOrderableGraphQLQueryable<TEntity, TProperty> OrderByDescending<TEntity, TProperty>(
            this IGraphQLQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> expression)
        {
            return (IOrderableGraphQLQueryable<TEntity, TProperty>)
                GraphQLQueryableMethods.GetOrderByDescendingMethod(typeof(TEntity), typeof(TProperty))
                    .Invoke(source, new object[] { expression })!;
        }

        public static IOrderableGraphQLQueryable<TEntity, TProperty> ThenBy<TEntity, TPreviousProperty, TProperty>(
            this IOrderableGraphQLQueryable<TEntity, TPreviousProperty> source, Expression<Func<TEntity, TProperty>> thenByExpression)
        {
            return (IOrderableGraphQLQueryable<TEntity, TProperty>)
                GraphQLQueryableMethods.GetThenByMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty))
                    .Invoke(source, new object[] { thenByExpression })!;
        }

        public static IOrderableGraphQLQueryable<TEntity, TProperty> ThenByDescending<TEntity, TPreviousProperty, TProperty>(
            this IOrderableGraphQLQueryable<TEntity, TPreviousProperty> source, Expression<Func<TEntity, TProperty>> thenByExpression)
        {
            return (IOrderableGraphQLQueryable<TEntity, TProperty>)
                GraphQLQueryableMethods.GetThenByDescendingMethod(typeof(TEntity), typeof(TPreviousProperty), typeof(TProperty))
                    .Invoke(source, new object[] { thenByExpression })!;
        }
    }
}
