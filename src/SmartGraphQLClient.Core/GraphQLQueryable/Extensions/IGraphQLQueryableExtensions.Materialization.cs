using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.GraphQLQueryable.Utils;
using SmartGraphQLClient.Exceptions;

namespace SmartGraphQLClient
{
    // TODO: Scalars
    public static partial class IGraphQLQueryableExtensions
    {
        #region ToArrayAsync
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as an array of entities
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An array of entities</returns>
        public static Task<TEntity[]> ToArrayAsync<TEntity>(this IGraphQLQueryable<TEntity> source)
            => source.ToArrayAsync(default!, default);
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as an array of entities
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An array of entities</returns>
        public static Task<TEntity[]> ToArrayAsync<TEntity>(this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config)
            => source.ToArrayAsync(config, default);
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as an array of entities
        /// </summary>
        /// <param name="source"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An array of entities</returns>
        public static Task<TEntity[]> ToArrayAsync<TEntity>(this IGraphQLQueryable<TEntity> source, CancellationToken token)
            => source.ToArrayAsync(default!, token);
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as an array of entities
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An array of entities</returns>
        public static Task<TEntity[]> ToArrayAsync<TEntity>(
            this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config, CancellationToken token)
        {
            return (Task<TEntity[]>)
                GraphQLQueryableMethods.GetToArrayAsyncMethod(typeof(TEntity))
                    .Invoke(source, new object[] { config, token })!;
        }

        #endregion

        #region ToListAsync
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a list of entities
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A list of entities</returns>
        public static Task<List<TEntity>> ToListAsync<TEntity>(this IGraphQLQueryable<TEntity> source)
            => source.ToListAsync(default!, default);
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a list of entities
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A list of entities</returns>
        public static Task<List<TEntity>> ToListAsync<TEntity>(this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config)
            => source.ToListAsync(config, default);
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a list of entities
        /// </summary>
        /// <param name="source"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A list of entities</returns>
        public static Task<List<TEntity>> ToListAsync<TEntity>(this IGraphQLQueryable<TEntity> source, CancellationToken token)
            => source.ToListAsync(default!, token);
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a list of entities
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A list of entities</returns>
        public static Task<List<TEntity>> ToListAsync<TEntity>(
            this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config, CancellationToken token)
        {
            return (Task<List<TEntity>>)
                GraphQLQueryableMethods.GetToListAsyncMethod(typeof(TEntity))
                    .Invoke(source, new object[] { config, token })!;
        }

        #endregion

        #region ToPageAsync

        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a collection-segment of entities
        /// </summary>
        /// <remarks>Good for requests with pagination</remarks>
        /// <param name="source"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A collection segment of entities with information about page and total count</returns>
        public static Task<CollectionSegment<TEntity>> ToPageAsync<TEntity>(this IGraphQLQueryable<TEntity> source)
            => source.ToPageAsync(default!, default);

        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a collection-segment of entities
        /// </summary>
        /// <remarks>Good for requests with pagination</remarks>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A collection segment of entities with information about page and total count</returns>
        public static Task<CollectionSegment<TEntity>> ToPageAsync<TEntity>(this IGraphQLQueryable<TEntity> source,
            GraphQLRequestConfiguration config)
            => source.ToPageAsync(config, default);

        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a collection-segment of entities
        /// </summary>
        /// <remarks>Good for requests with pagination</remarks>
        /// <param name="source"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A collection segment of entities with information about page and total count</returns>
        public static Task<CollectionSegment<TEntity>> ToPageAsync<TEntity>(this IGraphQLQueryable<TEntity> source,
            CancellationToken token)
            => source.ToPageAsync(default!, token);
        
        /// <summary>
        /// Makes HTTP-request to GraphQL-server and returns response as a collection-segment of entities
        /// </summary>
        /// <remarks>Good for requests with pagination</remarks>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>A collection segment of entities with information about page and total count</returns>
        public static Task<CollectionSegment<TEntity>> ToPageAsync<TEntity>(
            this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config, CancellationToken token)
        {
            return (Task<CollectionSegment<TEntity>>)
                GraphQLQueryableMethods.GetToPageAsyncMethod(typeof(TEntity))
                    .Invoke(source, new object[] { config, token })!;
        }

        #endregion

        #region FirstOrDefaultAsync

        /// <summary>
        /// Makes HTTP-request and returns entity or null
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity or null</returns>
        public static Task<TEntity?> FirstOrDefaultAsync<TEntity>(this IGraphQLQueryable<TEntity> source)
            => source.FirstOrDefaultAsync(default!, default);
        
        /// <summary>
        /// Makes HTTP-request and returns entity or null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity or null</returns>
        public static Task<TEntity?> FirstOrDefaultAsync<TEntity>(this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config)
            => source.FirstOrDefaultAsync(config, default);
        
        /// <summary>
        /// Makes HTTP-request and returns entity or null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity or null</returns>
        public static Task<TEntity?> FirstOrDefaultAsync<TEntity>(this IGraphQLQueryable<TEntity> source, CancellationToken token)
            => source.FirstOrDefaultAsync(default!, token);
        
        /// <summary>
        /// Makes HTTP-request and returns entity or null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config"></param>
        /// <param name="token">Additional request-configuration</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity or null</returns>
        public static Task<TEntity?> FirstOrDefaultAsync<TEntity>(
            this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config, CancellationToken token)
        {
            return (Task<TEntity?>)
                GraphQLQueryableMethods.GetFirstOrDefaultAsyncMethod(typeof(TEntity))
                    .Invoke(source, new object[] { config, token })!;
        }

        #endregion

        #region FirstAsync
        
        /// <summary>
        /// Makes HTTP-request and returns entity or throws exception if entity was null
        /// </summary>
        /// <param name="source"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity</returns>
        public static Task<TEntity> FirstAsync<TEntity>(this IGraphQLQueryable<TEntity> source)
            => source.FirstAsync(default!, default);
        
        /// <summary>
        /// Makes HTTP-request and returns entity or throws exception if entity was null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity</returns>
        public static Task<TEntity> FirstAsync<TEntity>(this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config)
            => source.FirstAsync(config, default);
        
        /// <summary>
        /// Makes HTTP-request and returns entity or throws exception if entity was null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="token"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity</returns>
        public static Task<TEntity> FirstAsync<TEntity>(this IGraphQLQueryable<TEntity> source, CancellationToken token)
            => source.FirstAsync(default!, token);

        /// <summary>
        /// Makes HTTP-request and returns entity or throws exception if entity was null
        /// </summary>
        /// <param name="source"></param>
        /// <param name="config">Additional request-configuration</param>
        /// <param name="token"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="GraphQLException"></exception>
        /// <returns>An entity</returns>
        public static Task<TEntity> FirstAsync<TEntity>(
            this IGraphQLQueryable<TEntity> source, GraphQLRequestConfiguration config, CancellationToken token)
        {
            return (Task<TEntity>)
                GraphQLQueryableMethods.GetFirstAsyncMethod(typeof(TEntity))
                    .Invoke(source, new object[] { config, token })!;
        }

        #endregion
    }
}
