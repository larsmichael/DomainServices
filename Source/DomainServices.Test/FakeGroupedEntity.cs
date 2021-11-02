namespace DomainServices.Test
{
    using System.Text.Json.Serialization;
    using System;
    using Abstractions;

    public class FakeGroupedEntity : BaseGroupedEntity<string>
    {
        public FakeGroupedEntity(string name, string group)
            : base(Guid.NewGuid().ToString(), name, group)
        {
        }

        [JsonConstructor]
        public FakeGroupedEntity(string id, string name, string group)
            : base(id, name, group)
        {
        }
    }
}