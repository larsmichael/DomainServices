namespace DomainServices.Test
{
    using System.Collections.Generic;
    using DomainServices.Authorization;
    using System;
    using Abstractions;

    public class FakeImmutableEntity : BaseNamedEntity<Guid>
    {
        public FakeImmutableEntity(Guid id, string name, IDictionary<string, object> metadata = null, IList<Permission> permissions = null)
            : base(id, name, metadata, permissions)
        {
        }
    }
}