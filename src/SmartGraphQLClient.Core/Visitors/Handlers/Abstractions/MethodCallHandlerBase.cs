using System.Linq.Expressions;
using SmartGraphQLClient.Core.Visitors.Handlers.Models;

namespace SmartGraphQLClient.Core.Visitors.Handlers.Abstractions;

public abstract class MethodCallHandlerBase : IMethodCallHandler
{
    public abstract bool TryHandle(MethodCallExpression expression, out MethodCallHandlerResult result);
}