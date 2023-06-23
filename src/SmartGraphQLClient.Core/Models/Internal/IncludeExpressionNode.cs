using System.Linq.Expressions;

namespace SmartGraphQLClient.Core.Models.Internal
{
    public class IncludeExpressionNode
    {
        public IncludeExpressionNode(
            LambdaExpression expression)
        {
            RootExpression = expression;
        }

        public LambdaExpression RootExpression { get; }
        public List<IncludeExpressionNode> Nodes { get; private set; } = new();

        internal IncludeExpressionNode Clone()
        {
            var nodes = Nodes.Select(x => x.Clone()).ToList();
            return new(RootExpression) { Nodes = nodes };
        }
    }
}
