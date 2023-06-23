namespace SmartGraphQLClient.Tests.TestsInfrastructure.Entities
{
    public class Child
    {
        public int Age { get; set; }
        public string Type { get; set; } = string.Empty;
        public int? Summary { get; set; }
        public int[] Array { get; set; } = System.Array.Empty<int>();
        public bool IsStarted { get; set; }
        public bool? IsCompleted { get; set; }

        public Child SubChild { get; set; } = new();
        public Child? SubChildNullable { get; set; }
        public Violation[] Violations { get; set; } = System.Array.Empty<Violation>();
    }
}
