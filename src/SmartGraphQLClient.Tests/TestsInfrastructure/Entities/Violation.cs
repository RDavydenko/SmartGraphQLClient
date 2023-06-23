using System;

namespace SmartGraphQLClient.Tests.TestsInfrastructure.Entities
{
    public class Violation
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;

        public Worker HeadOfWorkers { get; set; } = new();
        public Worker[] Workers { get; set; } = Array.Empty<Worker>();
    }
}
