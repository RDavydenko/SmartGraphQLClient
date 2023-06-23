using SmartGraphQLClient.Attributes;
using SmartGraphQLClient.Tests.TestsInfrastructure.Entities.Enums;
using System;
using System.Collections.Generic;

namespace SmartGraphQLClient.Tests.TestsInfrastructure.Entities
{
    [GraphQLEndpoint("entities")]
    public class TestEntity
    {
        public TestEntity()
        {

        }

        public TestEntity(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public int? MaybeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Child Child { get; set; } = new();
        public Child? ChildNullable { get; set; }
        public int[] Array { get; set; } = System.Array.Empty<int>();
        public bool IsStarted { get; set; }
        public bool? IsCompleted { get; set; }
        public SomethingEnum State { get; set; }
        public SomethingEnum? PreviousState { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        [GraphQLIgnore]
        public int AnotherSimpleProperty { get; set; }

        public Violation[] ChildrenArray { get; set; }
        public Violation[]? ChildrenArrayNullable { get; set; }
        public List<Violation> ChildrenList { get; set; }
        public List<Violation>? ChildrenListNullable { get; set; }
        public IEnumerable<Violation> ChildrenEnumerable { get; set; }
        public IEnumerable<Violation>? ChildrenEnumerableNullable { get; set; }
        public ICollection<Violation> ChildrenCollection { get; set; }
        public ICollection<Violation>? ChildrenCollectionNullable { get; set; }
        public HashSet<Violation> ChildrenHashSet { get; set; }
        public HashSet<Violation>? ChildrenHashSetNullable { get; set; }
    }
}
