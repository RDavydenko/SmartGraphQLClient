using Microsoft.Extensions.DependencyInjection;
using SmartGraphQLClient.Core.GraphQLQueryBuilder.Models;
using SmartGraphQLClient.Core.Models.Constants;
using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Providers.Abstractions;
using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.GraphQLQueryBuilder
{
    internal class GraphQLQueryBuilder
    {
        private readonly GraphQLQueryBuilderConfiguration _configuration;
        private IGraphQLEndpointProvider? _endpointProvider;
        private IGraphQLWhereStringProvider? _whereStringProvider;
        private IGraphQLOrderStringProvider? _orderStringProvider;
        private IGraphQLBodyStringProvider? _bodyStringProvider;
        private IGraphQLOffsetPaginationStringProvider? _offsetPaginationStringProvider;
        private IGraphQLValueFormatProvider? _valueFormatProvider;

        private GraphQLQueryBuilder(
            GraphQLQueryBuilderConfiguration config)
        {
            _configuration = config;
        }

        public static GraphQLQueryBuilder New(GraphQLQueryBuilderConfiguration config)
            => new(config);

        public string Build(out GraphQLRequestMetadataConfiguration config)
        {
            var endpoint = GetEndpoint();

            var parameters = BuildArguments();
            var body = BuildBody(out var rootSelector);

            config = new(_configuration.RootEntityType, rootSelector, _configuration.CallChain);

            return @$"
{{ 
    {endpoint} {(parameters.Count > 0 ? $"( {string.Join("\n", parameters)} ) " : "")} 
    {{
        {body}
    }}
}}";
        }

        internal void ConfigureRequestOptions(Action<GraphQLRequestConfiguration> configure)
        {
            _configuration.RequestConfiguration.ClearInternal();
            configure(_configuration.RequestConfiguration);
        }

        private string GetEndpoint()
        {
            var endpoint = _configuration.RequestConfiguration.Endpoint;
            if (endpoint is null)
            {
                _endpointProvider ??= _configuration.ServiceProvider.GetRequiredService<IGraphQLEndpointProvider>();
                endpoint = _endpointProvider.GetGraphQLEndpoint(_configuration.RootEntityType);
            }

            return endpoint;
        }

        private List<string> BuildArguments()
        {
            var arguments = new List<string>();

            var whereArguments = BuildWhereString();
            if (whereArguments is not null) arguments.Add(whereArguments);

            var orderArgument = BuildOrderString();
            if (orderArgument is not null) arguments.Add(orderArgument);

            var skipArgument = BuildSkipString();
            if (skipArgument is not null) arguments.Add(skipArgument);

            var takeArgument = BuildTakeString();
            if (takeArgument is not null) arguments.Add(takeArgument);

            EnsureArguments(arguments);

            return arguments;
        }

        private string? BuildWhereString()
        {
            _whereStringProvider ??= _configuration.ServiceProvider.GetRequiredService<IGraphQLWhereStringProvider>();
            return _whereStringProvider.Build(_configuration.RootEntityType, _configuration.CallChain.QueryConditions);
        }

        private string? BuildOrderString()
        {
            _orderStringProvider ??= _configuration.ServiceProvider.GetRequiredService<IGraphQLOrderStringProvider>();
            return _orderStringProvider.Build(_configuration.RootEntityType, _configuration.CallChain.QueryOrders);
        }

        private string BuildBody(out LambdaExpression rootSelector)
        {
            _bodyStringProvider ??= _configuration.ServiceProvider.GetRequiredService<IGraphQLBodyStringProvider>();
            (var body, rootSelector) = _bodyStringProvider.Build(
                _configuration.RootEntityType,
                _configuration.CallChain.QuerySelector,
                _configuration.CallChain.QueryIncludes,
                new()
                {
                    DisabledIgnoreAttributes = (bool)_configuration.QueryableConfiguration
                        .GetValueOrDefault(QueryableConfigurationKeys.DisabledIgnoreAttributes, false)!
                });
            _configuration.CallChain.ChangeQuerySelector(rootSelector);

            /* CollectionPage */
            if (_configuration.RequestConfiguration.IsCollectionPage)
            {
                const string keyword = "items";
                const string pageInfo = "pageInfo { hasNextPage hasPreviousPage }";
                const string totalCount = "totalCount";

                return $"{keyword} {{\n{body}\n}}\n{pageInfo}\n{totalCount}";
            }
            /* One */
            else if (_configuration.RequestConfiguration.IsFirstOrDefault)
            {
                return body;
            }
            /* Array */
            else if (_configuration.RequestConfiguration.IsArray)
            {
                return body;
            }
            else
            {
                return body;
            }
        }

        private string? BuildSkipString()
        {
            _offsetPaginationStringProvider ??= _configuration.ServiceProvider.GetRequiredService<IGraphQLOffsetPaginationStringProvider>();
            return _offsetPaginationStringProvider.BuildSkip(_configuration.CallChain.QuerySkip);
        }

        private string? BuildTakeString()
        {
            _offsetPaginationStringProvider ??= _configuration.ServiceProvider.GetRequiredService<IGraphQLOffsetPaginationStringProvider>();
            return _offsetPaginationStringProvider.BuildTake(_configuration.CallChain.QueryTake);
        }

        private void EnsureArguments(List<string> arguments)
        {
            if (!_configuration.CallChain.QueryArguments.Any())
            {
                return;
            }

            _valueFormatProvider ??= _configuration.ServiceProvider.GetRequiredService<IGraphQLValueFormatProvider>();

            foreach (var arg in _configuration.CallChain.QueryArguments)
            {
                arguments.Add($"{arg.Key}: {_valueFormatProvider.GetFormattedValue(arg.Value)}");
            }
        }
    }
}
