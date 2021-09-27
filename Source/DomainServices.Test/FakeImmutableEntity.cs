namespace DomainServices.Test
{
    using System;
    using Abstractions;

    [Serializable]
    public class FakeImmutableEntity : BaseNamedEntity<Guid>
    {
        public FakeImmutableEntity(Guid id, string name) : base(id, name)
        {
        }
    }
}