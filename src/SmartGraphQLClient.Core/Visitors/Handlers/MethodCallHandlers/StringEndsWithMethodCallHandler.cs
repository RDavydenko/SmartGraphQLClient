using System.Linq.Expressions;
using System.Reflection;
using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Models.Constants;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Handlers.Abstractions;
using SmartGraphQLClient.Core.Visitors.Handlers.Models;

namespace SmartGraphQLClient.Core.Visitors.Handlers.MethodCallHandlers;

internal sealed class StringEndsWithMethodCallHandler : MethodCallHandlerBase
{
    private static readonly MethodInfo StringEndsWithMethod = typeof(string)
        .GetMethods(BindingFlags.Instance | BindingFlags.Public)
        .First(x => x.Name == "EndsWith" && x.GetParameters().Length == 1);

    public override bool TryHandle(MethodCallExpression expression, out MethodCallHandlerResult result)
    {
        result = MethodCallHandlerResult.Failed();

        if (expression.Arguments.Count != 1) return false;
        if (expression.Object is null || !ExpressionHelper.TryExtractMemberExpression(expression.Object, out var memberExpression)) return false;
        if (!ExpressionHelper.TryExtractConstantExpression(expression.Arguments[0], out var constantExpression)) return false;
        if (expression.Method != StringEndsWithMethod) return false;

        result = MethodCallHandlerResult.Success(
            memberExpression, 
            constantExpression.Value, 
            GraphQLOperations.EndsWith,
            GraphQLOperations.NotEndsWith);
        return true;
    }
}