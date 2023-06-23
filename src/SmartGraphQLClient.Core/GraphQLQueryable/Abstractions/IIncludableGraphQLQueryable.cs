namespace SmartGraphQLClient.Core.GraphQLQueryable.Abstractions
{
    public interface IIncludableGraphQLQueryable<out TEntity, out TProperty>
        : IGraphQLQueryable<TEntity>
    {
    }
}
