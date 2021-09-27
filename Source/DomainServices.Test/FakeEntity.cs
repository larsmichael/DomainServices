namespace DomainServices.Test
{
    using System;
    using Abstractions;

    [Serializable]
    public class FakeEntity : BaseGroupedEntity<string>
    {
        public FakeEntity(string id, string name, string group=null) : base(id, name, group)
        {
        }
    }
}