using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Models.Internal;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartGraphQLClient.Core.Utils
{
    internal static class LocalCallChainEvaluator
    {
        public static object EvaluateSingle(object value, List<LocalExpression> expressions)
        {
            var arrayWithValue = CreateArray(value);
            var evaluatedArray = EvaluateArrayInternal(
                arrayWithValue,
                expressions,
                new[]
                {
                    LocalExpression.ExpressionType.Condition,
                    LocalExpression.ExpressionType.Skip,
                    LocalExpression.ExpressionType.Take,
                });

            var elementType = evaluatedArray.GetType().GetCollectionElementType()!;
            return ExecuteFirst(evaluatedArray, elementType);
        }
        public static object EvaluateArray(object array, List<LocalExpression> expressions)
        {
            return EvaluateArrayInternal(array, expressions);
        }

        private static object EvaluateArrayInternal(
            object array, List<LocalExpression> expressions, LocalExpression.ExpressionType[]? forbiddenTypes = null)
        {
            forbiddenTypes ??= Array.Empty<LocalExpression.ExpressionType>();

            if (!array.GetType().IsCollectionType())
            {
                throw new NotImplementedException("Supports only collection types");
            }
            
            var elementType = array.GetType().GetCollectionElementType()!;
            object enumerable = array;

            foreach (var expression in expressions)
            {
                if (forbiddenTypes.Contains(expression.Type))
                {
                    throw new InvalidOperationException($"Operation {expression.Type} is forbid");
                }

                enumerable = expression.Type switch
                {
                    LocalExpression.ExpressionType.Selector => ExecuteSelect(enumerable, elementType, expression.Selector!, out elementType),
                    LocalExpression.ExpressionType.Condition => ExecuteCondition(enumerable, elementType, expression.Condition!),
                    LocalExpression.ExpressionType.Order => ExecuteOrder(enumerable, elementType, expression.OrderExpression!),
                    LocalExpression.ExpressionType.Take => ExecuteTake(enumerable, elementType, expression.TakeCount!.Value),
                    LocalExpression.ExpressionType.Skip => ExecuteSkip(enumerable, elementType, expression.SkipCount!.Value),
                    _ => throw new NotImplementedException($"ExpressionType {expression.Type} is not implemented")
                };
            }

            return ExecuteToArray(enumerable, elementType);
        }

        private static object ExecuteSelect(
            object enumerable, Type typeIn, LambdaExpression lambdaExpression, out Type newType)
        {
            var compiledDelegate = lambdaExpression.Compile();
            newType = lambdaExpression.ReturnType;
            return LinqMethods.GetSelectMethod(typeIn, newType).Invoke(null, new[] { enumerable, compiledDelegate })!;
        }

        private static object ExecuteCondition(
            object enumerable, Type typeIn, LambdaExpression lambdaExpression)
        {
            var compiledDelegate = lambdaExpression.Compile();
            return LinqMethods.GetWhereMethod(typeIn).Invoke(null, new[] { enumerable, compiledDelegate })!;
        }

        private static object ExecuteOrder(
            object enumerable, Type typeIn, OrderExpression orderExpression)
        {
            var compiledDelegate = orderExpression.Expression.Compile();
            var propertyType = compiledDelegate.GetMethodInfo().GetGenericArguments()[1];
            if (orderExpression.Direction == OrderDirection.ASC)
            {
                return LinqMethods.GetOrderByMethod(typeIn, propertyType).Invoke(null, new[] { enumerable, compiledDelegate })!;
            }

            return LinqMethods.GetOrderByDescendingMethod(typeIn, propertyType).Invoke(null, new[] { enumerable, compiledDelegate })!;
        }

        private static object ExecuteTake(
            object enumerable, Type typeIn, int count)
        {
            return LinqMethods.GetTakeMethod(typeIn).Invoke(null, new[] { enumerable, count })!;
        }

        private static object ExecuteSkip(
            object enumerable, Type typeIn, int count)
        {
            return LinqMethods.GetSkipMethod(typeIn).Invoke(null, new[] { enumerable, count })!;
        }

        private static object ExecuteToArray(
            object enumerable, Type typeIn)
        {
            return LinqMethods.GetToArrayMethod(typeIn).Invoke(null, new[] { enumerable })!;
        }

        private static object ExecuteFirst(
            object enumerable, Type typeIn)
        {
            return LinqMethods.GetFirstMethod(typeIn).Invoke(null, new[] { enumerable })!;
        }

        private static object CreateArray(object value)
        {
            var array = Array.CreateInstance(value.GetType(), 1);
            array.SetValue(value, 0);
            return array;
        }
    }
}
