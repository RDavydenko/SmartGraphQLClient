namespace SmartGraphQLClient.Core.Visitors.SelectExpressionVisitor.Models
{
    record class FieldNode(string Name)
    {
        public readonly List<FieldNode> Nodes = new();

        public override string ToString()
        {
            if (Nodes.Count == 0) return Name;

            return $"{Name} {{ {string.Join(" ", Nodes)} }}";
        }
    }
}
