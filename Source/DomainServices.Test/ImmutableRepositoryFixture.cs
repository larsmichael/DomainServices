namespace DomainServices.Test
{
    using System.Linq;
    using AutoFixture;

    public class ImmutableRepositoryFixture
    {
        public ImmutableRepositoryFixture()
        {
            var fixture = new Fixture();
            var fakeEntityList = fixture.CreateMany<FakeImmutableEntity>().ToList();
            Repository = new FakeImmutableRepository(fakeEntityList);
            RepeatCount = fixture.RepeatCount;
        }

        public FakeImmutableRepository Repository { get; }

        public int RepeatCount { get; }
    }
}