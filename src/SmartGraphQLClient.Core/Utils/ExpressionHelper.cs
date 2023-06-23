using System.Linq.Expressions;
using System.Reflection;

namespace SmartGraphQLClient.Core.Utils
{
    internal static class ExpressionHelper
    {
        public static bool TryExtractConstantExpression(Expression expression, out ConstantExpression constant)
        {
            constant = ExtractConstantExpression(expression)!;

            return constant is not null;
        }

        public static ConstantExpression? ExtractConstantExpression(Expression expression)
        {
            // x => x.Id == 5
            if (expression is ConstantExpression constantExpression) return constantExpression;

            // x => x.EnumValue == SomethingEnum.APPROVED
            if (expression is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            {
                return ExtractConstantExpression(unary.Operand);
            }

            // x => x.Id == instance.Id
            if (expression is MemberExpression callerMemberExpression)
            {
                object? closedInstance = null;
                var path = new Stack<MemberInfo>();
                path.Push(callerMemberExpression.Member);
                Expression? current = callerMemberExpression.Expression;
                while (true)
                {
                    if (current is null)
                    {
                        break;
                    }
                    if (current is ConstantExpression currentConstant)
                    {
                        closedInstance = currentConstant.Value;
                        break;
                    }
                    else if (current is MemberExpression memberExpression)
                    {
                        path.Push(memberExpression.Member);
                        current = memberExpression.Expression;
                    }
                    else if (current is NewExpression currentNewExpression)
                    {
                        var instance = ExtractConstantExpression(currentNewExpression);
                        if (instance is null) return null;
                        closedInstance = instance.Value;
                        break;
                    }
                    else
                    {
                        return null;
                    }
                }

                var extractedValue = ReflectionHelper.GetValueByPath(closedInstance, path);
                return Expression.Constant(extractedValue);
            }

            // x => x.Id == instance.GetValue()
            if (expression is MethodCallExpression callerMethodCallExpression)
            {
                var method = callerMethodCallExpression.Method;
                var arguments = callerMethodCallExpression.Arguments;
                object? closedInstance = null;
                var path = new Stack<MemberInfo>();
                Expression? current = callerMethodCallExpression.Object;
                while (true)
                {
                    if (current is null)
                    {
                        break;
                    }
                    if (current is ConstantExpression currentConstant)
                    {
                        closedInstance = currentConstant.Value;
                        break;
                    }
                    else if (current is MemberExpression memberExpression)
                    {
                        path.Push(memberExpression.Member);
                        current = memberExpression.Expression;
                    }
                    else if (current is NewExpression currentNewExpression)
                    {
                        var instance = ExtractConstantExpression(currentNewExpression);
                        if (instance is null) return null;
                        closedInstance = instance.Value;
                        break;
                    }
                    else
                    {
                        return null;
                    }
                }

                var instanceForInvoke = ReflectionHelper.GetValueByPath(closedInstance, path);
                var args = new object?[arguments.Count];
                for (int i = 0; i < args.Length; i++)
                {
                    var arg = ExtractConstantExpression(arguments[i]);
                    if (arg is null) return null;
                    args[i] = arg.Value;
                }
                return Expression.Constant(method.Invoke(instanceForInvoke, args));
            }

            // x => new[] { 1, 2, 3 }.Contains(x.Id)
            if (expression is NewArrayExpression newArrayExpression)
            {
                var array = Array.CreateInstance(TypeSystem.GetElementType(newArrayExpression.Type), newArrayExpression.Expressions.Count);
                int i = 0;
                foreach (var constant in newArrayExpression.Expressions.Select(ExtractConstantExpression))
                {
                    if (constant is null) return null;

                    array.SetValue(constant.Value, i);
                    i++;
                }

                return Expression.Constant(array);
            }

            // x => new SomethingClass()
            if (expression is NewExpression newExpression)
            {
                var parameters = new object?[newExpression.Arguments.Count];
                int i = 0;
                foreach (var constant in newExpression.Arguments.Select(ExtractConstantExpression))
                {
                    if (constant is null) return null;

                    parameters[i] = constant.Value;
                    i++;
                }
                var instance = newExpression.Constructor!.Invoke(parameters);
                return Expression.Constant(instance);
            }

            return null;
        }

        public static bool TryExtractMemberExpression(Expression expression, out MemberExpression member)
        {
            member = ExtractMemberExpression(expression)!;

            return member is not null;
        }

        public static MemberExpression? ExtractMemberExpression(Expression expression)
        {
            if (expression is MemberExpression memberExpression)
            {
                return memberExpression;
            }
            if (expression is UnaryExpression { NodeType: ExpressionType.Convert } unary)
            {
                return ExtractMemberExpression(unary.Operand);
            }

            return null;
        }

        /// <summary>
        /// Returns reversed properties path
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        /// <remarks>x => x.Child.Age returns [Age, Child]</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public static List<MemberInfo> GetReversedPropertyPath(MemberExpression prop, out bool wasMethodCall)
        {
            var path = new List<MemberInfo>() { prop.Member };
            var expr = prop.Expression;
            wasMethodCall = false;

            while (true)
            {
                if (expr is ParameterExpression)
                {
                    break;
                }
                else if (expr is MemberExpression memberExpression)
                {
                    path.Add(memberExpression.Member);
                    expr = memberExpression.Expression;
                }
                else if (expr is MethodCallExpression)
                {
                    wasMethodCall = true;
                    break;
                }
                else
                {
                    ArgumentNullException.ThrowIfNull(expr);
                    
                    throw new NotImplementedException($"Expression {expr.GetType().Name} is not supported");
                }
            }

            return path;
        }

        /// <summary>
        /// Returns properties path
        /// </summary>
        /// <param name="prop"></param>
        /// <param name="wasMethodCall"></param>
        /// <returns></returns>
        /// <remarks>x => x.Child.Age returns [Child, Age]</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public static List<MemberInfo> GetPropertyPath(MemberExpression prop, out bool wasMethodCall)
        {
            var result = GetReversedPropertyPath(prop, out wasMethodCall);
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Returns sequence expressions for call chain
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <remarks>x => x.Child.Ages.Select() returns [MemberExpression (Child), MemberExpression (Ages), MethodCallExpression (Select)]</remarks>
        /// <exception cref="NotImplementedException"></exception>
        public static List<Expression> GetSequenceCallChain(Expression expression)
        {
            var sequence = new List<Expression>() { expression };

            while (true)
            {
                if (expression is ParameterExpression)
                {
                    break;
                }
                else if (expression is MemberExpression memberExpression)
                {
                    if (memberExpression.Expression is null) break;
                    if (memberExpression.Expression is ParameterExpression) break;

                    sequence.Add(memberExpression.Expression);
                    expression = memberExpression.Expression;
                }
                else if (expression is MethodCallExpression methodCallExpression)
                {
                    if (methodCallExpression.Object is not null)
                    {
                        sequence.Add(methodCallExpression.Object);
                        expression = methodCallExpression.Object;
                    }
                    else
                    {
                        if (methodCallExpression.Arguments.Count == 0) break;
                        sequence.Add(methodCallExpression.Arguments[0]);
                        expression = methodCallExpression.Arguments[0];
                    }
                }
                else if (expression is ConstantExpression)
                {
                    break;
                }
                else
                {
                    throw new NotImplementedException($"Expression {expression.GetType().Name} is not supported");
                }
            }

            sequence.Reverse();
            return sequence;
        }
    }
}
