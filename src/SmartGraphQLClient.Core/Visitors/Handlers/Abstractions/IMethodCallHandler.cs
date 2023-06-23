using System.Linq.Expressions;
using SmartGraphQLClient.Core.Visitors.Handlers.Models;

namespace SmartGraphQLClient.Core.Visitors.Handlers.Abstractions;

public interface IMethodCallHandler
{
    bool TryHandle(MethodCallExpression expression, out MethodCallHandlerResult result);
}