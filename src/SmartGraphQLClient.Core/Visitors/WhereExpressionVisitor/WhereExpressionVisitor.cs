using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Abstractions;
using System.Linq.Expressions;
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
