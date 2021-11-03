namespace DomainServices.Test
{
    using DomainServices.Authorization;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using System;
    using Abstractions;

    public class FakeGroupedEntity : BaseGroupedEntity<string>
    {
        public FakeGroupedEntity(string name, string group, IDictionary<string, object> metadata = null, IList<Permission> permissions = null)
            : base(Guid.NewGuid().ToString(), name, group, metadata, permissions)
        {
        }

        [JsonConstructor]
        public FakeGroupedEntity(string id, string name, string group, IDictionary<string, object> metadata = null, IList<Permission> permissions = null)
            : base(id, name, group, metadata, permissions)
        {
        }
    }
}