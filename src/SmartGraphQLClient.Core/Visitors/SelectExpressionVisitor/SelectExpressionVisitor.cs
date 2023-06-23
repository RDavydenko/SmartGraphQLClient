using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Abstractions;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor
{
    internal class SelectExpressionVisitor : SelectExpressionVisitorBase
    {
        private readonly LambdaExpression _expression;

        internal SelectExpressionVisitor(
            LambdaExpression expression,
            IServiceProvider serviceProvider,
            SelectExpressionVisitorConfiguration configuration)
            : base(serviceProvider, configuration)
        {
            _expression = expression;
        }

        public override void Visit()
        {
            _root = new("");
            _rootType ??= _expression.Type;
            Visit(_expression.Body, _root);
        }

        private void Visit(Expression expression, FieldNode node, bool throwError = true)
        {
            switch (expression)
            {
                case NewExpression newExpression:
                    VisitNewExpression(newExpression, node);
                    break;
                case MemberInitExpression memberInitExpression:
                    VisitMemberInitExpression(memberInitExpression, node);
                    break;
                case LambdaExpression lambdaExpression when !ReferenceEquals(_root, node):
                    Visit(lambdaExpression.Body, node);
                    break;
                case MemberExpression memberExpression:
                    VisitMemberExpression(memberExpression, node);
                    break;
                case ConditionalExpression conditionalExpression:
                    VisitConditionalExpression(conditionalExpression, node);
                    break;
                case BinaryExpression binaryExpresion:
                    VisitBinaryExpression(binaryExpresion, node);
                    break;
                case UnaryExpression unaryExpression:
                    VisitUnaryExpression(unaryExpression, node);
                    break;
                case MethodCallExpression methodCallExpression:
                    VisitMethodCallExpression(methodCallExpression, node);
                    break;
                case ParameterExpression parameterExpression:
                    VisitParameterExpression(parameterExpression, node);
                    break;
                default:
                    if (throwError)
                    {
                        throw new NotImplementedException($"Expression {expression.GetType().Name} is not implemented");
                    }
                    break;
            }
        }

        private void VisitNewExpression(NewExpression expression, FieldNode node)
        {
            foreach (var argument in expression.Arguments)
            {
                VisitInternalExpression(argument, node);
            }
        }

        private void VisitMemberInitExpression(MemberInitExpression expression, FieldNode node)
        {
            foreach (var expr in expression.Bindings)
            {
                if (expr is MemberAssignment memberAssignment)
                {
                    VisitInternalExpression(memberAssignment.Expression, node);
                }
                else
                {
                    throw new NotImplementedException($"Expression {expr.GetType().Name} is not implemented");
                }
            }
        }

        private void VisitInternalExpression(Expression expression, FieldNode node)
        {
            if (ExpressionHelper.TryExtractMemberExpression(expression, out var memberExpression))
            {
                VisitMemberExpression(memberExpression, node);
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                VisitMethodCallExpression(methodCallExpression, node);
            }
            else
            {
                Visit(expression, node);
            }
        }

        private void VisitMethodCallExpression(MethodCallExpression methodCallExpression, FieldNode node)
        {
            var sequence = ExpressionHelper.GetSequenceCallChain(methodCallExpression);
            VisitMethodCallChain(methodCallExpression, node, sequence);
        }

        private void VisitConditionalExpression(ConditionalExpression conditionalExpression, FieldNode node)
        {
            Visit(conditionalExpression.IfTrue, node, throwError: false);
            Visit(conditionalExpression.IfFalse, node, throwError: false);
            Visit(conditionalExpression.Test, node, throwError: false);
        }

        private void VisitUnaryExpression(UnaryExpression unaryExpression, FieldNode node)
        {
            Visit(unaryExpression.Operand, node, throwError: false);
        }

        private void VisitBinaryExpression(BinaryExpression binaryExpresion, FieldNode node)
        {
            Visit(binaryExpresion.Left, node, throwError: false);
            Visit(binaryExpresion.Right, node, throwError: false);
        }

        private void VisitParameterExpression(ParameterExpression parameterExpression, FieldNode node)
        {
            var type = parameterExpression.Type.IsCollectionType() 
                ? parameterExpression.Type.GetCollectionElementType()
                : parameterExpression.Type;

            if (type.IsSimpleType() || type.IsCollectionType())
            {
                throw new ArgumentException("Parameter's type must not be simple-type or collection-type");
            }

            VisitAllSimpleMembers(type, node);
        }

        private static bool IsSelectMethodCall(MethodCallExpression expression, out MemberExpression memberExpression, out Expression internalExpression)
        {
            internalExpression = default!;
            memberExpression = default!;

            if (expression.Arguments.Count != 2) return false;
            if (!ExpressionHelper.TryExtractMemberExpression(expression.Arguments[0], out memberExpression)) return false;
            if (expression.Arguments[1] is not LambdaExpression lambdaExpression) return false;
            internalExpression = lambdaExpression;

            if (!memberExpression.Type.IsCollectionType()) return false;

            var inElementType = memberExpression.Type.GetCollectionElementType();
            var outElementType = lambdaExpression.ReturnType;

            var selectMethod = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(m => m.Name == "Select" && m.IsGenericMethod && m.GetParameters().Length == 2)
                .MakeGenericMethod(inElementType, outElementType);

            if (selectMethod != expression.Method) return false;

            return true;
        }

        private void VisitMemberExpression(MemberExpression expression, FieldNode node)
        {
            var sequence = ExpressionHelper.GetSequenceCallChain(expression);
            var path = sequence.TakeWhile(e => e is MemberExpression).OfType<MemberExpression>().ToList();

            if (sequence.Count != path.Count)
            {
                VisitMethodCallChain(expression, node, sequence);
                return;
            }

            var currentNode = node;
            for (int i = 0; i < path.Count; i++)
            {
                var memberInfo = path[i].Member;
                currentNode = CreateNode(currentNode, memberInfo);

                // on last iteration if not ends on method call
                if (i == path.Count - 1)
                {
                    var type = memberInfo.GetMemberInfoType();
                    if (!type.IsSimpleType())
                    {
                        VisitAllSimpleMembers(type, currentNode);
                    }
                }
            }
        }

        private void VisitMethodCallChain(Expression expression, FieldNode node, List<Expression> chain)
        {
            if (chain.Count >= 2 &&
                chain[1] is MethodCallExpression methodCallExpression &&
                IsSelectMethodCall(methodCallExpression, out var selectMemberExpression, out var selectInternalExpression))
            {
                var currentNode = CreateNode(node, selectMemberExpression);
                Visit(selectInternalExpression, currentNode);
            }
            else if (chain.Count >= 1 &&
                     chain[0] is MemberExpression rootMemberExpression)
            {
                var lastMemberExpression = (MemberExpression)chain.TakeWhile(e => e is MemberExpression).Last();
                VisitMemberExpression(lastMemberExpression, node);
            }
            else if (chain.Count > 0 &&
                     chain[0] is not ConstantExpression) // ignore closure
            {
                throw new NotImplementedException("Not implemented case");
            }
        }

        internal override LambdaExpression GetSelector() => _expression;
    }
}
