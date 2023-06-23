namespace SmartGraphQLClient.Tests.TestsInfrastructure.Entities
{
    public class Worker
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? Age { get; set; }
        public bool IsActive { get; set; }

        public Worker? Chief { get; set; }
    }
}
