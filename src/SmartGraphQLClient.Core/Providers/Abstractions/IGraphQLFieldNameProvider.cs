using System.Reflection;

namespace SmartGraphQLClient.Core.Providers.Abstractions
{
    public interface IGraphQLFieldNameProvider
    {
        string GetFieldName(MemberInfo member);
    }
}
