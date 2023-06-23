using System.Linq.Expressions;
using System.Reflection;
using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Models.Constants;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Handlers.Abstractions;
using SmartGraphQLClient.Core.Visitors.Handlers.Models;

namespace SmartGraphQLClient.Core.Visitors.Handlers.MethodCallHandlers;

internal sealed class AnyMethodCallHandlerEmpty : MethodCallHandlerBase
{
    private static readonly MethodInfo NonGenericEnumerableAnyMethod =
        typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(x => x.Name == "Any" && x.GetParameters().Length == 1);

    public override bool TryHandle(MethodCallExpression expression, out MethodCallHandlerResult result)
    {
        result = MethodCallHandlerResult.Failed();

        if (expression.Arguments.Count != 1) return false;
        if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out var memberExpression)) return false;

        if (!memberExpression.Type.TryGetCollectionElementType(out var elementType)) return false;

        var anyMethod = NonGenericEnumerableAnyMethod.MakeGenericMethod(elementType);

        if (expression.Method != anyMethod) return false;

        // any: true / any: false
        result = MethodCallHandlerResult.Success(
            memberExpression,
            (bool inverted) => !inverted ? true : false,
            GraphQLOperations.Any,
            GraphQLOperations.Any);
        
        return true;
    }
}