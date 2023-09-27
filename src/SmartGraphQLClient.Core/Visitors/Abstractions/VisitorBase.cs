using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using SmartGraphQLClient.Core.Providers.Abstractions;
using System.Reflection;
using SmartGraphQLClient.Core.Models.Internal;

namespace SmartGraphQLClient.Core.Visitors.Abstractions
{
    internal abstract class VisitorBase
    {
        private readonly IGraphQLFieldNameProvider _fieldNameProvider;
        private readonly IGraphQLValueFormatProvider _valueFormatProvider;

        protected VisitorBase(
            IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            _fieldNameProvider = serviceProvider.GetRequiredService<IGraphQLFieldNameProvider>();
            _valueFormatProvider = serviceProvider.GetRequiredService<IGraphQLValueFormatProvider>();
        }

        protected IServiceProvider ServiceProvider { get; }

        protected string FormatFieldName(MemberInfo memberInfo)
            => _fieldNameProvider.GetFieldName(memberInfo);
        
        protected string FormatValue(object? value)
            => _valueFormatProvider.GetFormattedValue(value);
    }
}
