namespace DomainServices.Test
{
    using System;
    using Abstractions;

    public class FakeEntity : BaseGroupedEntity<string>
    {
        public FakeEntity(string id, string name, string group=null, bool foo = true, DateTime bar = default) : base(id, name, group)
        {
            Foo = foo;
            Bar = bar;
        }

        public bool Foo { get; }

        public DateTime Bar { get; }
    }
}