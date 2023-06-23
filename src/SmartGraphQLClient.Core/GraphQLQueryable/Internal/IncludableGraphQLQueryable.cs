using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.Models.Internal;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.GraphQLQueryable.Internal
{
    internal class IncludableGraphQLQueryable<TEntity, TProperty> :
        GraphQLQueryable<TEntity>,
        IIncludableGraphQLQueryable<TEntity, TProperty>
    {
        internal GraphQLQueryable<TEntity> Queryable { get; private set; }
        internal IncludeExpressionNode CurrentNode { get; private set; }

        public IncludableGraphQLQueryable(
            GraphQLQueryable<TEntity> queryable,
            IncludeExpressionNode currentNode)
            : base(queryable)
        {
            Queryable = queryable;
            CurrentNode = currentNode;
        }

        protected IIncludableGraphQLQueryable<TEntity, TOutProperty> ThenInclude<TOutProperty>(
            Expression<Func<TProperty, TOutProperty>> thenIncludeExpression)
        {
            var newIncludeNode = CurrentNode.Nodes.FirstOrDefault(x => x.RootExpression == thenIncludeExpression);
            if (newIncludeNode is null)
            {
                newIncludeNode = new IncludeExpressionNode(thenIncludeExpression);
                CurrentNode.Nodes.Add(newIncludeNode);
            }

            return new IncludableGraphQLQueryable<TEntity, TOutProperty>(
                Queryable,
                newIncludeNode);
        }
    }

    internal static class IncludableGraphQLQueryableHelper
    {
        public static IncludableGraphQLQueryable<TEntity, TProperty> ConvertTo<TEntity, TPreviousProperty, TProperty>(
            IncludableGraphQLQueryable<TEntity, TPreviousProperty> source)
        {
            return new IncludableGraphQLQueryable<TEntity, TProperty>(
                source.Queryable,
                source.CurrentNode);
        }
    }
}