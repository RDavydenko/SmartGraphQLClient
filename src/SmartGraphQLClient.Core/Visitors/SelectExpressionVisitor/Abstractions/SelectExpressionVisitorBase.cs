using SmartGraphQLClient.Attributes;
using SmartGraphQLClient.Core.Extensions;
using SmartGraphQLClient.Core.Models.Internal;
using SmartGraphQLClient.Core.Utils;
using SmartGraphQLClient.Core.Visitors.Abstractions;
using SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models;
using System.Linq.Expressions;
using System.Reflection;

namespace SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Abstractions
{
    internal abstract class SelectExpressionVisitorBase : VisitorBase
    {
        protected Type _rootType = default!;
        protected FieldNode _root = new("");
        protected readonly SelectExpressionVisitorConfiguration _configuration;

        public SelectExpressionVisitorBase(
            IServiceProvider serviceProvider,
            SelectExpressionVisitorConfiguration configuration)
            : base(serviceProvider)
        {
            _configuration = configuration;
        }

        public abstract void Visit();

        public virtual void VisitIncludeExpressions(List<IncludeExpressionNode> includes)
        {
            foreach (var include in includes)
            {
                VisitIncludeExpression(include, _root);
            }
        }

        private void VisitIncludeExpression(IncludeExpressionNode include, FieldNode node)
        {
            if (include.RootExpression.Body is not MemberExpression rootMemberExpression)
            {
                throw new ArgumentException($"Include's {nameof(include.RootExpression)} must be MemberExpression", nameof(include));
            }

            var currentNode = CreateNode(node, rootMemberExpression);
            VisitAllSimpleMembers(rootMemberExpression.Type, currentNode);

            foreach (var childInclude in include.Nodes)
            {
                VisitIncludeExpression(childInclude, currentNode);
            }
        }

        internal Type GetResultType() => _rootType;
        internal abstract LambdaExpression GetSelector();

        protected FieldNode CreateNode(FieldNode node, MemberExpression memberExpression)
        {
            var path = ExpressionHelper.GetPropertyPath(memberExpression, out var wasMethodCall);
            if (wasMethodCall)
            {
                throw new NotImplementedException("Method call is not supported on properties' path");
            }
            var currentNode = node;

            foreach (var member in path)
            {
                currentNode = CreateNode(currentNode, member);
            }

            return currentNode;
        }

        protected FieldNode CreateNode(FieldNode node, MemberInfo member)
        {
            var formattedName = FormatFieldName(member);
            var result = node.Nodes.FirstOrDefault(x => x.Name == formattedName);
            if (result is null)
            {
                result = new(formattedName);
                node.Nodes.Add(result);
            }

            return result;
        }

        protected void VisitAllSimpleMembers(Type type, FieldNode node)
        {
            if (type.IsCollectionType())
            {
                var elementType = type.GetCollectionElementType();
                if (!elementType.IsSimpleType())
                {
                    VisitAllSimpleMembers(elementType, node);
                    return;
                }
            }

            IEnumerable<MemberInfo> props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.PropertyType.IsSimpleType() || x.PropertyType.IsCollectionOfSimpleType())
                .Select(x => x);

            IEnumerable<MemberInfo> fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.FieldType.IsSimpleType() || x.FieldType.IsCollectionOfSimpleType())
                .Select(x => x);

            var members = props.Concat(fields);

            members = members
                .WhereIf(x => 
                {
                    var ignoreAttr = x.GetCustomAttribute<GraphQLIgnoreAttribute>(inherit: true);
                    if (ignoreAttr is null) return true;
                    return !ignoreAttr.Ignore;
                }, !_configuration.DisabledIgnoreAttributes);

            foreach (var member in members)
            {
                CreateNode(node, member);
            }
        }

        public override string ToString()
        {
            return string.Join(" ", _root.Nodes);
        }
    }
}