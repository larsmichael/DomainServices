namespace DomainServices.Test
{
    using DomainServices.Authorization;
    using System.Collections.Generic;
    using System;
    using Abstractions;

    public class FakeEntity : BaseGroupedEntity<string>
    {
        public FakeEntity(string id, string name, string group=null, IDictionary<string, object> metadata = null, IList<Permission> permissions = null, bool foo = true, DateTime bar = default)
            : base(id, name, group, metadata, permissions)
        {
            Foo = foo;
            Bar = bar;
        }

        public bool Foo { get; }

        public DateTime Bar { get; }
    }
}