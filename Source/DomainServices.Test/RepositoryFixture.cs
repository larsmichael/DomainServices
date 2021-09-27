namespace DomainServices.Test
{
    using System.Linq;
    using AutoFixture;
    using Repositories;

    public class RepositoryFixture
    {
        public RepositoryFixture()
        {
            var fixture = new Fixture();
            var fakeEntityList = fixture.CreateMany<FakeEntity>().ToList();
            Repository = new FakeGroupedRepository<FakeEntity, string>(fakeEntityList);
            RepeatCount = fixture.RepeatCount;
        }

        public FakeGroupedRepository<FakeEntity, string> Repository { get; }

        public int RepeatCount { get; }
    }
}