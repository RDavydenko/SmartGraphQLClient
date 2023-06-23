using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Models.Constants;
using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Providers.Abstractions;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Handlers.Abstractions;
using SmartGraphQLClient.Core.Visitors.Handlers.Models;

namespace SmartGraphQLClient.Core.Visitors.Handlers.MethodCallHandlers;

internal sealed class ContainsMethodCallHandler : MethodCallHandlerBase
{
    private static readonly MethodInfo NonGenericEnumerableContainsMethod =
        typeof(Enumerable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(x => x.Name == "Contains" && x.GetParameters().Length == 2);

    private readonly IGraphQLValueFormatProvider _valueFormatProvider;

    public ContainsMethodCallHandler(
        IGraphQLValueFormatProvider valueFormatProvider)
    {
        _valueFormatProvider = valueFormatProvider;
    }

    public override bool TryHandle(MethodCallExpression expression, out MethodCallHandlerResult result)
    {
        result = MethodCallHandlerResult.Failed();

        MemberExpression memberExpression = default!;
        ConstantExpression? constant = default!;

        if (expression.Arguments.Count == 1)
        {
            if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out memberExpression)) return false;
            if (expression.Object is null || !ExpressionHelper.TryExtractConstantExpression(expression.Object, out constant)) return false;
        }
        else if (expression.Arguments.Count == 2)
        {
            if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[1], out memberExpression)) return false;
            if (!ExpressionHelper.TryExtractConstantExpression(expression.Arguments[0], out constant)) return false;
        }
        else
        {
            return false;
        }

        var elementType = memberExpression.Type;

        var enumerableContainsMethod = NonGenericEnumerableContainsMethod.MakeGenericMethod(elementType);
        var hashSetContainsMethod = typeof(HashSet<>)
            .MakeGenericType(elementType)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .First(x => x.Name == "Contains" && x.GetParameters().Length == 1);
        var listContainsMethod = typeof(List<>)
            .MakeGenericType(elementType)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .First(x => x.Name == "Contains" && x.GetParameters().Length == 1);

        if (expression.Method != enumerableContainsMethod &&
            expression.Method != hashSetContainsMethod &&
            expression.Method != listContainsMethod)
        {
            return false;
        }

        var value = constant.Value is IEnumerable array
            ? new WrappedEnumerable(array, _valueFormatProvider)
            : null;
        
        result = MethodCallHandlerResult.Success(
            memberExpression,
            value,
            GraphQLOperations.In,
            GraphQLOperations.NotIn);
        
        return true;
    }
}