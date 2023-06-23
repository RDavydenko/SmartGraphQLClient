using System.Linq.Expressions;
using System.Reflection;
using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Models.Constants;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Handlers.Abstractions;
using SmartGraphQLClient.Core.Visitors.Handlers.Models;

namespace SmartGraphQLClient.Core.Visitors.Handlers.MethodCallHandlers;

internal sealed class ArrayContainsMethodCallHandler : MethodCallHandlerBase
{
    private static readonly MethodInfo NonGenericEnumerableContainsMethod =
        typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(x => x.Name == "Contains" && x.GetParameters().Length == 2);
    
    public override bool TryHandle(MethodCallExpression expression, out MethodCallHandlerResult result)
    {
        result = MethodCallHandlerResult.Failed();

        if (expression.Arguments.Count != 2) return false;
        if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out var memberExpression)) return false;
        if (!ExpressionHelper.TryExtractConstantExpression(expression.Arguments[1], out var constantExpression)) return false;

        if (!memberExpression.Type.TryGetCollectionElementType(out var elementType)) return false;

        var containsMethod = NonGenericEnumerableContainsMethod.MakeGenericMethod(elementType);
        if (expression.Method != containsMethod) return false;

        result = MethodCallHandlerResult.Success(
            memberExpression, 
            constantExpression.Value, 
            GraphQLOperations.Contains,
            GraphQLOperations.NotContains);
        return true;
    }
}