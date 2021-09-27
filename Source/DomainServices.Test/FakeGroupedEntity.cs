namespace DomainServices.Test
{
    using System;
    using Abstractions;
    using Newtonsoft.Json;

    [Serializable]
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