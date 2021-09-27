namespace DomainServices.Test
{
    using System.Linq;
    using AutoFixture;
    using AutoFixture.Xunit2;
    using Abstractions;
    using Repositories;

    internal class AutoFakeEntityDataAttribute : AutoDataAttribute
    {
        public AutoFakeEntityDataAttribute(int repeatCount = 0)
            : base(() =>
            {
                var fixture = new Fixture();
                if (repeatCount > 0)
                {
                    fixture.RepeatCount = repeatCount;
                    var fakeEntityList = fixture.CreateMany<FakeEntity>().ToList();

                    fixture.Register<IUpdatableRepository<FakeEntity, string>>(() => new FakeRepository<FakeEntity, string>(fakeEntityList));
                    fixture.Register<IGroupedRepository<FakeEntity>>(() => new FakeGroupedRepository<FakeEntity, string>(fakeEntityList));
                }
                else
                {
                    fixture.Register<IUpdatableRepository<FakeEntity, string>>(() => new FakeRepository<FakeEntity, string>());
                    fixture.Register<IGroupedRepository<FakeEntity>>(() => new FakeGroupedRepository<FakeEntity, string>());
                }

                return fixture;
            })
        {
        }
    }
}