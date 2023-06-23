using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.Models.Internal;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.GraphQLQueryable.Internal
{
    internal class OrderedGraphQLQueryable<TEntity, TProperty> :
        GraphQLQueryable<TEntity>,
        IOrderableGraphQLQueryable<TEntity, TProperty>
    {
        private GraphQLQueryable<TEntity> Queryable { get; set; }

        public OrderedGraphQLQueryable(
            GraphQLQueryable<TEntity> queryable)
            : base(queryable)
        {
            Queryable = queryable;
        }

        protected IOrderableGraphQLQueryable<TEntity, TNextProperty> ThenBy<TNextProperty>(
            Expression<Func<TEntity, TNextProperty>> thenByExpression)
            => ThenByDirection(thenByExpression, OrderDirection.ASC);

        protected IOrderableGraphQLQueryable<TEntity, TNextProperty> ThenByDescending<TNextProperty>(
            Expression<Func<TEntity, TNextProperty>> thenByExpression)
            => ThenByDirection(thenByExpression, OrderDirection.DESC);

        private IOrderableGraphQLQueryable<TEntity, TNextProperty> ThenByDirection<TNextProperty>(
            Expression<Func<TEntity, TNextProperty>> expression, OrderDirection direction)
        {
            var queryableClone = Queryable.Clone<TEntity>();
            queryableClone.CallChain.AddOrder(new(expression, direction));
            return new OrderedGraphQLQueryable<TEntity, TNextProperty>(queryableClone);
        }
    }
}