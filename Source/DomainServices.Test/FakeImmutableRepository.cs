namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using Abstractions;
    using Repositories;

    public class FakeImmutableRepository : FakeRepository<FakeImmutableEntity, Guid>, IImmutableRepository<FakeImmutableEntity>
    {
        public FakeImmutableRepository(IEnumerable<FakeImmutableEntity> entities) : base(entities)
        {
        }
    }
}