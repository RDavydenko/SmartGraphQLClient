using SmartGraphQLClient.Core.GraphQLQueryable.Abstractions;
using SmartGraphQLClient.Core.GraphQLQueryable.Internal;
using SmartGraphQLClient.Core.GraphQLQueryBuilder.Models;
using SmartGraphQLClient.Core.Models.Internal;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.GraphQLQueryable
{
    internal class GraphQLQueryable<TEntity> :
        IGraphQLQueryable<TEntity>
    {
        public Type GpaphQLClientType { get; }
        internal IServiceProvider ServiceProvider { get; private set; }
        internal HttpClient HttpClient { get; private set; }
        internal string? Endpoint { get; private set; }
        internal Type RootEntityType { get; private set; }
        internal GraphQLExpressionCallChainConfiguration CallChain { get; private set; } = new();
        internal Dictionary<string, object?> Configuration { get; private set; } = new();

        public GraphQLQueryable(
            Type gpaphQLClientType,
            IServiceProvider serviceProvider,
            HttpClient httpClient,
            string? endpoint = null)
        {
            GpaphQLClientType = gpaphQLClientType;
            ServiceProvider = serviceProvider;
            HttpClient = httpClient;
            Endpoint = endpoint;

            RootEntityType = typeof(TEntity);
        }

        internal GraphQLQueryable(
            GraphQLQueryable<TEntity> queryable)
            : this(
                  queryable.GpaphQLClientType,
                  queryable.ServiceProvider,
                  queryable.HttpClient,
                  queryable.Endpoint,
                  queryable.CallChain,
                  queryable.Configuration)
        {
        }

        private GraphQLQueryable(
            Type gpaphQLClientType,
            IServiceProvider serviceProvider,
            HttpClient httpClient,
            string? endpoint,
            GraphQLExpressionCallChainConfiguration callChain,
            Dictionary<string, object?> configuration)
            : this(gpaphQLClientType, serviceProvider, httpClient, endpoint)
        {
            CallChain = callChain;
            Configuration = configuration;
        }

        internal string DebugView
        {
            get
            {
                var queryBuilder = GetQueryBuilder();
                return queryBuilder.Build(out var _);
            }
        }

        protected IGraphQLQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var clone = Clone();
            clone.CallChain.AddCondition(predicate);
            return clone;
        }

        protected IGraphQLQueryable<TOutEntity> Select<TOutEntity>(Expression<Func<TEntity, TOutEntity>> selector)
        {
            var clone = Clone<TOutEntity>();
            clone.CallChain.AddSelector(selector);
            return clone;
        }

        protected IIncludableGraphQLQueryable<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> includeExpression)
        {
            var clone = Clone();
            var includeNode = clone.CallChain.QueryIncludes.FirstOrDefault(x => x.RootExpression == includeExpression);
            if (includeNode is null)
            {
                includeNode = new(includeExpression);
                clone.CallChain.AddInclude(includeNode);
            }

            return new IncludableGraphQLQueryable<TEntity, TProperty>(clone, includeNode);
        }

        protected IOrderableGraphQLQueryable<TEntity, TProperty> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> orderByExpression)
            => OrderByDirection(orderByExpression, OrderDirection.ASC);

        protected IOrderableGraphQLQueryable<TEntity, TProperty> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> orderByExpression)
            => OrderByDirection(orderByExpression, OrderDirection.DESC);

        private IOrderableGraphQLQueryable<TEntity, TProperty> OrderByDirection<TProperty>(Expression<Func<TEntity, TProperty>> expression, OrderDirection direction)
        {
            var clone = Clone();
            clone.CallChain.AddOrder(new(expression, direction));
            return new OrderedGraphQLQueryable<TEntity, TProperty>(clone);
        }

        protected IGraphQLQueryable<TEntity> Skip(int count)
        {
            var clone = Clone();
            clone.CallChain.AddSkip(count);
            return clone;
        }

        protected IGraphQLQueryable<TEntity> Take(int count)
        {
            var clone = Clone();
            clone.CallChain.AddTake(count);
            return clone;
        }

        protected IGraphQLQueryable<TEntity> Argument(string key, object? value)
        {
            var clone = Clone();
            clone.CallChain.AddArgument(new(key, value));
            return clone;
        }

        protected Task<TEntity[]> ToArrayAsync(GraphQLRequestConfiguration config, CancellationToken token)
        {
            var queryBuilder = GetQueryBuilder(config);
            var executor = GetRequestExecutor(queryBuilder);
            return executor.ExecuteToArrayAsync<TEntity>(token);
        }

        protected async Task<List<TEntity>> ToListAsync(GraphQLRequestConfiguration config, CancellationToken token)
        {
            var array = await ToArrayAsync(config, token);
            return array.ToList();
        }

        protected Task<CollectionSegment<TEntity>> ToPageAsync(GraphQLRequestConfiguration config, CancellationToken token)
        {
            var queryBuilder = GetQueryBuilder(config);
            var executor = GetRequestExecutor(queryBuilder);
            return executor.ExecuteToPageAsync<TEntity>(token);
        }

        protected Task<TEntity?> FirstOrDefaultAsync(GraphQLRequestConfiguration config, CancellationToken token)
        {
            var queryBuilder = GetQueryBuilder(config);
            var executor = GetRequestExecutor(queryBuilder);
            return executor.ExecuteFirstOrDefaultAsync<TEntity>(token);
        }

        protected async Task<TEntity> FirstAsync(GraphQLRequestConfiguration config, CancellationToken token)
        {
            var entity = await FirstOrDefaultAsync(config, token);
            return entity ?? throw new InvalidOperationException("Entity is null");
        }
        
        protected IGraphQLQueryable<TEntity> Configure(string key, object? value)
        {
            var clone = Clone();
            clone.Configuration[key] = value;
            return clone;
        }

        private GraphQLQueryBuilder.GraphQLQueryBuilder GetQueryBuilder(
            GraphQLRequestConfiguration? config = null)
        {
            if (Endpoint is not null && config?.Endpoint is null)
            {
                config ??= new GraphQLRequestConfiguration();
                config.Endpoint = Endpoint;
            }

            return GraphQLQueryBuilder.GraphQLQueryBuilder.New(
                            new GraphQLQueryBuilderConfiguration(
                                RootEntityType,
                                CallChain,
                                Configuration,
                                ServiceProvider,
                                config));
        }

        private GraphQLRequestExecutor.GraphQLRequestExecutor GetRequestExecutor(
            GraphQLQueryBuilder.GraphQLQueryBuilder queryBuilder)
        {
            return GraphQLRequestExecutor.GraphQLRequestExecutor.New(
                ServiceProvider,
                HttpClient,
                queryBuilder,
                GpaphQLClientType
                );
        }

        internal GraphQLQueryable<TEntity> Clone()
            => Clone<TEntity>();

        internal GraphQLQueryable<TOutEntity> Clone<TOutEntity>()
        {
            var clone = new GraphQLQueryable<TOutEntity>(
                GpaphQLClientType,
                ServiceProvider,
                HttpClient,
                Endpoint,
                CallChain.Clone(),
                new(Configuration))
            {
                RootEntityType = this.RootEntityType
            };

            return clone;
        }
    }
}
