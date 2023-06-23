using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Abstractions;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SmartGraphQLClient.Core.Visitors.Handlers.Abstractions;
using SmartGraphQLClient.Core.Visitors.Handlers.Models;

namespace SmartGraphQLClient.Core.Visitors.WhereExpressionVisitor
{
    internal class WhereExpressionVisitor : VisitorBase
    {
        private readonly LambdaExpression _expression;
        private StringBuilder _sb = new();
        private IEnumerable<IMethodCallHandler>? _methodCallHandlers;

        internal WhereExpressionVisitor(
            LambdaExpression expression,
            IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            _expression = expression;
        }

        public void Visit()
        {
            _sb = new StringBuilder();
            Visit(_expression.Body);
        }

        private void Visit(Expression expression)
        {
            switch (expression)
            {
                case BinaryExpression binaryExpression:
                    VisitBinaryExpression(binaryExpression);
                    break;
                case MethodCallExpression methodCallExpression:
                    VisitMethodCallExpression(methodCallExpression);
                    break;
                case MemberExpression memberExpression:
                    VisitMemberExpression(memberExpression);
                    break;
                case UnaryExpression unaryExpression:
                    VisitUnaryExpression(unaryExpression);
                    break;
                default:
                    throw new NotImplementedException($"Expression {expression.GetType().Name} is not implemented");
            }
        }

        private void VisitUnaryExpression(UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType == ExpressionType.Not)
            {
                switch (unaryExpression.Operand)
                {
                    case MethodCallExpression methodCallExpression:
                        VisitMethodCallExpression(methodCallExpression, not: true);
                        break;
                    case MemberExpression memberExpression:
                        VisitMemberExpression(memberExpression, not: true);
                        break;
                    case BinaryExpression binaryExpression:
                        VisitBinaryExpression(binaryExpression, not: true);
                        break;
                    default:
                        throw new NotImplementedException($"Expression {unaryExpression.Operand.GetType().Name} is not implemented");
                }
            }
        }

        private void VisitMemberExpression(MemberExpression memberExpression, bool not = false)
        {
            // if nullable
            if (memberExpression.Expression?.Type != null &&
                Nullable.GetUnderlyingType(memberExpression.Expression.Type) != null)
            {
                Func<string, int, int, bool>? filter = null;
                var hasValue = memberExpression.Member == memberExpression.Expression.Type.GetMember("HasValue")[0];
                hasValue = !not ? hasValue : !hasValue;
                var op = hasValue ? "neq" : "eq";
                filter = (_, index, __) => index != 0; // Удаляем первый (HasValue)

                AppendMemberNameWithCondition(memberExpression, $"{{ {op}: null }}", filter);
                return;
            }

            if (memberExpression.Type == typeof(bool))
            {
                var op = !not ? "eq" : "neq";
                AppendMemberNameWithCondition(memberExpression, $"{{ {op}: true }}");
                return;
            }

            throw new NotImplementedException($"Invalid case for {nameof(VisitMemberExpression)}");
        }

        private void VisitMethodCallExpression(MethodCallExpression expression, bool not = false)
        {
            _methodCallHandlers ??= ServiceProvider.GetRequiredService<IEnumerable<IMethodCallHandler>>();

            var canHandle = false;
            MethodCallHandlerResult handleResult = default!;

            foreach (var handler in _methodCallHandlers)
            {
                if (handler.TryHandle(expression, out handleResult))
                {
                    canHandle = true;
                    break;
                }
            }

            if (!canHandle)
            {
                throw new NotImplementedException($"MethodCall {expression.Method.Name} is not implemented");
            }

            if (handleResult.ResultType == MethodCallHandlerResultType.MemberWithValue ||
                handleResult.ResultType == MethodCallHandlerResultType.MemberWithValueFactory)
            {
                var value = handleResult.ResultType == MethodCallHandlerResultType.MemberWithValueFactory
                    ? handleResult.ValueFactory(not)
                    : handleResult.Value;
                
                AppendMemberNameWithCondition(handleResult.MemberExpression, $"{{ {handleResult.GetOperator(not)}: {FormatValue(value)} }}");
                return;
            }

            if (handleResult.ResultType == MethodCallHandlerResultType.MemberWithInternalExpression)
            {
                var internalVisitor = new WhereExpressionVisitor(handleResult.InternalExpression, ServiceProvider);
                internalVisitor.Visit();
                var internalCondition = internalVisitor.ToString(); // TODO: Придумать что-то, чтобы передавать sb
                
                AppendMemberNameWithCondition(handleResult.MemberExpression, $"{{ {handleResult.GetOperator(not)}: {{ {internalCondition} }} }}");
                return;
            }

            throw new NotImplementedException($"MethodCallHandlerResult.ResultType {handleResult.ResultType} is not implemented");
        }

        private bool IsArrayContainsMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out object? value)
        {
            value = null;
            memberExpression = default!;

            if (expression.Arguments.Count != 2) return false;
            if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out memberExpression)) return false;
            if (!ExpressionHelper.TryExtractConstantExpression(expression.Arguments[1], out var constantExpression)) return false;

            var elementType = memberExpression.Type.GetCollectionElementType();

            var containsMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "Contains" && x.GetParameters().Length == 2)
                .MakeGenericMethod(elementType);

            if (containsMethod != expression.Method)
            {
                return false;
            }

            value = constantExpression.Value;
            return true;
        }

        private bool IsAnyMethodCall(MethodCallExpression expression, out MemberExpression memberExpression)
        {
            memberExpression = default!;

            if (expression.Arguments.Count != 1) return false;
            if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out memberExpression)) return false;

            var elementType = memberExpression.Type.GetCollectionElementType();

            var containsMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "Any" && x.GetParameters().Length == 1)
                .MakeGenericMethod(elementType);

            if (containsMethod != expression.Method)
            {
                return false;
            }

            return true;
        }

        private bool IsAnyMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out LambdaExpression internalExpression)
        {
            memberExpression = default!;
            internalExpression = default!;

            if (expression.Arguments.Count != 2) return false;
            if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out memberExpression)) return false;
            if (expression.Arguments[1] is not LambdaExpression _internalExpression) return false;
            internalExpression = _internalExpression;

            // TODO: Перед каждым вызовом GetCollectionElementType проверку на IsCollection
            var elementType = memberExpression.Type.GetCollectionElementType();

            var containsMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "Any" && x.GetParameters().Length == 2)
                .MakeGenericMethod(elementType);

            if (containsMethod != expression.Method)
            {
                return false;
            }

            return true;
        }

        private bool IsAllMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out LambdaExpression internalExpression)
        {
            memberExpression = default!;
            internalExpression = default!;

            if (expression.Arguments.Count != 2) return false;
            if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out memberExpression)) return false;
            if (expression.Arguments[1] is not LambdaExpression _internalExpression) return false;
            internalExpression = _internalExpression;

            var elementType = memberExpression.Type.GetCollectionElementType();

            var containsMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "All" && x.GetParameters().Length == 2)
                .MakeGenericMethod(elementType);

            if (containsMethod != expression.Method)
            {
                return false;
            }

            return true;
        }

        private bool IsStringContainsMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out object? value)
        {
            value = null;
            memberExpression = default!;

            if (expression.Arguments.Count != 1) return false;
            if (expression.Object is null || !ExpressionHelper.TryExtractMemberExpression(expression.Object, out memberExpression)) return false;
            if (!ExpressionHelper.TryExtractConstantExpression(expression.Arguments[0], out var constantExpression)) return false;

            var containsMethod = typeof(string)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.Name == "Contains" && x.GetParameters().Length == 1);

            if (containsMethod != expression.Method)
            {
                return false;
            }

            value = constantExpression.Value;
            return true;
        }

        private bool IsStringStartsWithMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out object? value)
        {
            value = null;
            memberExpression = default!;

            if (expression.Arguments.Count != 1) return false;
            if (expression.Object is null || !ExpressionHelper.TryExtractMemberExpression(expression.Object, out memberExpression)) return false;
            if (!ExpressionHelper.TryExtractConstantExpression(expression.Arguments[0], out var constantExpression)) return false;

            var containsMethod = typeof(string)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.Name == "StartsWith" && x.GetParameters().Length == 1);

            if (containsMethod != expression.Method)
            {
                return false;
            }

            value = constantExpression.Value;
            return true;
        }

        private bool IsStringEndsWithMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out object? value)
        {
            value = null;
            memberExpression = default!;

            if (expression.Arguments.Count != 1) return false;
            if (expression.Object is null || !ExpressionHelper.TryExtractMemberExpression(expression.Object, out memberExpression)) return false;
            if (!ExpressionHelper.TryExtractConstantExpression(expression.Arguments[0], out var constantExpression)) return false;

            var containsMethod = typeof(string)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.Name == "EndsWith" && x.GetParameters().Length == 1);

            if (containsMethod != expression.Method)
            {
                return false;
            }

            value = constantExpression.Value;
            return true;
        }

        private bool IsContainsMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out object? value)
        {
            value = null;
            memberExpression = default!;
            ConstantExpression? constant = null;

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

            var elementType = memberExpression.Type.GetCollectionElementType();

            var enumerableContainsMethod = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(x => x.Name == "Contains" && x.GetParameters().Length == 2)
                .MakeGenericMethod(elementType);
            var hashSetContainsMethod = typeof(HashSet<>)
                .MakeGenericType(elementType)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.Name == "Contains" && x.GetParameters().Length == 1);
            var listContainsMethod = typeof(List<>)
                .MakeGenericType(elementType)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.Name == "Contains" && x.GetParameters().Length == 1);

            if (enumerableContainsMethod != expression.Method &&
                hashSetContainsMethod != expression.Method &&
                listContainsMethod != expression.Method)
            {
                return false;
            }

            value = constant.Value is IEnumerable array 
                ? CreateWrappedEnumerable(array) 
                : null;
            return true;
        }

        private void VisitBinaryExpression(BinaryExpression expression, bool not = false)
        {
            if (expression.NodeType == ExpressionType.AndAlso)
            {
                ThrowIfTrue(not, "And operation is not supported with inverting");
                VisitAndExpression(expression);
            }
            else if (expression.NodeType == ExpressionType.OrElse)
            {
                ThrowIfTrue(not, "Or operation is not supported with inverting");
                VisitOrExpression(expression);
            }
            else
            {
                VisitOperationExpression(expression, not);
            }
        }

        private void VisitAndExpression(BinaryExpression expression)
        {
            _sb.AppendLine("and: [ ");

            _sb.Append("{ ");
            Visit(expression.Left);
            _sb.AppendLine(" }");

            _sb.Append("{ ");
            Visit(expression.Right);
            _sb.AppendLine(" }");

            _sb.AppendLine(" ]");
        }

        private void VisitOrExpression(BinaryExpression expression)
        {
            _sb.AppendLine("or: [ ");

            _sb.Append("{ ");
            Visit(expression.Left);
            _sb.AppendLine(" }");

            _sb.Append("{ ");
            Visit(expression.Right);
            _sb.AppendLine(" }");

            _sb.AppendLine(" ]");
        }

        private void VisitOperationExpression(BinaryExpression expression, bool not = false)
        {
            var (left, right, initialOp) = (expression.Left, expression.Right, expression.NodeType);
            var (prop, val, op) = GetPropertyAccessAndValue(left, right, initialOp);
            op = not ? LogicalInvertOpertaror(op) : op;
            switch (op)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                    var condition = $"{{ {FormatOperator(op)}: {FormatValue(val)} }}";
                    AppendMemberNameWithCondition(prop, condition);
                    break;
                default:
                    throw new NotImplementedException($"ExpressionType {op} is not implemented");
            }
        }

        private void AppendMemberNameWithCondition(
            MemberExpression expression,
            string condition,
            Func<string, int, int, bool>? filter = null)
        {
            var propertyPath = ExpressionHelper.GetReversedPropertyPath(expression, out var wasMethodCall)
                .Select((memberInfo, i) => new { Path = memberInfo, Index = i })
                .ToList();
            if (wasMethodCall)
            {
                throw new NotImplementedException("Method call is not supported on properties' path");
            }
            if (filter != null) propertyPath = propertyPath.Where(x => filter(x.Path.Name, x.Index, propertyPath.Count)).ToList();

            var sb = new StringBuilder($"{FormatFieldName(propertyPath[0].Path)}: {condition}");
            foreach (var memberName in propertyPath.Skip(1))
            {
                sb.Append(" }");
                sb.Insert(0, $"{FormatFieldName(memberName.Path)}: {{ ");
            }
            _sb.Append(sb);
        }

        private (MemberExpression prop, object? val, ExpressionType op) GetPropertyAccessAndValue(
            Expression left, Expression right, ExpressionType op)
        {
            MemberExpression? prop;
            object? val = null;
            ExpressionType resOp = op;

            // TODO: Добавить поддержку констант, которые легко вычислить, например SomethingEnum.APPROVED.ToString()
            //       Как вариант вычислять тут и не зависеть от ConstantExpression совсем

            if (TryParseParseAccessAndValue(left, right, out prop, out val))
            {
                return (prop, val, resOp);
            }
            else if (TryParseParseAccessAndValue(right, left, out prop, out val))
            {
                resOp = ExpandOperator(op);
                return (prop, val, resOp);
            }
            else
            {
                throw new NotImplementedException($"{left.GetType().Name} and {right.GetType().Name} are not implemented for {nameof(GetPropertyAccessAndValue)}");
            }
        }

        private bool TryParseParseAccessAndValue(Expression member, Expression constant, out MemberExpression memberExpression, out object? value)
        {
            value = null;
            if (!ExpressionHelper.TryExtractMemberExpression(member, out memberExpression)) return false;
            if (!ExpressionHelper.TryExtractConstantExpression(constant, out var constantExpression)) return false;

            value = constantExpression.Value;

            if (memberExpression.Type.IsEnum)
            {
                value = value is not null
                    ? Enum.ToObject(memberExpression.Type, value)
                    : value;
            }

            return true;
        }

        private static string FormatOperator(ExpressionType op)
        {
            return op switch
            {
                ExpressionType.Equal => "eq",
                ExpressionType.NotEqual => "neq",
                ExpressionType.GreaterThan => "gt",
                ExpressionType.GreaterThanOrEqual => "gte",
                ExpressionType.LessThan => "lt",
                ExpressionType.LessThanOrEqual => "lte",
                _ => throw new NotImplementedException($"ExpressionType {op} is not implemented")
            };
        }

        // Разворот оператора (> -> <)
        private static ExpressionType ExpandOperator(ExpressionType op)
        {
            return op switch
            {
                ExpressionType.GreaterThan => ExpressionType.LessThan,
                ExpressionType.GreaterThanOrEqual => ExpressionType.LessThanOrEqual,
                ExpressionType.LessThan => ExpressionType.GreaterThan,
                ExpressionType.LessThanOrEqual => ExpressionType.GreaterThanOrEqual,
                _ => op
            };
        }

        // Логическое инвертирование оператора (eq -> neq)
        private static ExpressionType LogicalInvertOpertaror(ExpressionType op)
        {
            return op switch
            {
                ExpressionType.Equal => ExpressionType.NotEqual,
                ExpressionType.NotEqual => ExpressionType.Equal,
                ExpressionType.GreaterThan => ExpressionType.LessThanOrEqual,
                ExpressionType.GreaterThanOrEqual => ExpressionType.LessThan,
                ExpressionType.LessThan => ExpressionType.GreaterThanOrEqual,
                ExpressionType.LessThanOrEqual => ExpressionType.GreaterThan,
                _ => throw new NotImplementedException($"ExpressionType {op} is not implemented")
            };
        }

        public override string ToString()
        {
            return _sb.ToString();
        }

        private static void ThrowIfTrue(bool value, string message = "An error occured")
        {
            if (value)
            {
                throw new Exception(message);
            }
        }
    }
}
