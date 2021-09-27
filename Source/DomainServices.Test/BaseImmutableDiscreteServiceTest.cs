namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture.Xunit2;
    using Abstractions;
    using DomainServices.Logging;
    using Xunit;

    public class BaseImmutableDiscreteServiceTest : IClassFixture<ImmutableRepositoryFixture>
    {
        private readonly ImmutableDiscreteService _service;
        private readonly int _repeatCount;

        public BaseImmutableDiscreteServiceTest(ImmutableRepositoryFixture fixture)
        {
            var repository = fixture.Repository;
            _service = new ImmutableDiscreteService(repository);
            _repeatCount = fixture.RepeatCount;
        }

        [Fact]
        public void CreateWithNullRepositoryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ImmutableDiscreteService(null));
        }

        [Fact]
        public void GetNonExistingThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => _service.Get(Guid.NewGuid()));
        }

        [Fact]
        public void RemoveNonExistingThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => _service.Remove(Guid.NewGuid()));
        }

        [Theory, AutoData]
        public void AddAndGetIsOk(FakeImmutableEntity entity)
        {
            _service.Add(entity);
            Assert.Equal(entity.Id, _service.Get(entity.Id).Id);
        }

        [Theory, AutoData]
        public void GetManyIsOk(FakeImmutableEntity[] entities)
        {
            foreach (var entity in entities)
            {
                _service.Add(entity);
            }

            var myEntities = _service.Get(entities.Select(e => e.Id)).ToArray();
            Assert.Equal(_repeatCount, myEntities.Length);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
        }

        [Theory, AutoData]
        public void GetManyWithNonExistingIdIsOk(FakeImmutableEntity[] entities)
        {
            foreach (var entity in entities)
            {
                _service.Add(entity);
            }

            var ids = entities.Select(e => e.Id).ToList();
            var nonExistingGuid = Guid.NewGuid();
            ids.Add(nonExistingGuid);
            var myEntities = _service.Get(ids).ToArray();
            Assert.Equal(_repeatCount, myEntities.Length);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
            Assert.DoesNotContain(nonExistingGuid, myEntities.Select(e => e.Id));
        }

        [Theory, AutoData]
        public void GetManyWithNonExistingIdLogsWarning(FakeImmutableEntity[] entities)
        {
            var repository = new FakeImmutableRepository(entities);

            var logger = new FakeLogRepository();
            var service = new ImmutableDiscreteService(repository, logger);
            var ids = entities.Select(e => e.Id).ToList();
            var nonExistingGuid = Guid.NewGuid();
            ids.Add(nonExistingGuid);
            var myEntities = service.Get(ids).ToArray();
            Assert.Equal(_repeatCount, myEntities.Length);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
            Assert.DoesNotContain(nonExistingGuid, myEntities.Select(e => e.Id));
            var query = new Query<LogEntry>(new QueryCondition("LogLevel", LogLevel.Warning));
            var logEntries = logger.Get(query).ToArray();
            Assert.Single(logEntries);
            Assert.Contains($"'DomainServices.Test.FakeImmutableEntity' with id '{nonExistingGuid}' was not found.", logEntries.Select(l => l.Text));
        }

        [Fact]
        public void GetAllIsOk()
        {
            Assert.True(_service.GetAll().Any());
        }

        [Fact]
        public void GetIdsIsOk()
        {
            Assert.True(_service.GetIds().Any());
        }

        [Fact]
        public void CountIsOk()
        {
            Assert.True(_service.Count() > 0);
        }

        [Fact]
        public void ExistsIsOk()
        {
            var entity = _service.GetAll().ToArray()[0];
            Assert.True(_service.Exists(entity.Id));
        }

        [Fact]
        public void DoesNotExistsIsOk()
        {
            Assert.False(_service.Exists(Guid.NewGuid()));
        }

        [Theory, AutoData]
        public void RemoveIsOk(FakeImmutableEntity entity)
        {
            _service.Add(entity);
            _service.Remove(entity.Id);

            Assert.Throws<KeyNotFoundException>(() => _service.Get(entity.Id));
            Assert.False(_service.Exists(entity.Id));
        }

        [Theory, AutoData]
        public void EventsAreRaisedOnAdd(FakeImmutableEntity entity)
        {
            var raisedEvents = new List<string>();
            _service.Adding += (s, e) => { raisedEvents.Add("Adding"); };
            _service.Added += (s, e) => { raisedEvents.Add("Added"); };

            _service.Add(entity);

            Assert.Equal("Adding", raisedEvents[0]);
            Assert.Equal("Added", raisedEvents[1]);
        }

        [Theory, AutoData]
        public void EventsAreRaisedOnRemove(FakeImmutableEntity entity)
        {
            var raisedEvents = new List<string>();
            _service.Deleting += (s, e) => { raisedEvents.Add("Deleting"); };
            _service.Deleted += (s, e) => { raisedEvents.Add("Deleted"); };
            _service.Add(entity);

            _service.Remove(entity.Id);

            Assert.Equal("Deleting", raisedEvents[0]);
            Assert.Equal("Deleted", raisedEvents[1]);
        }

        private class ImmutableDiscreteService : BaseImmutableDiscreteService<FakeImmutableEntity>
        {
            public ImmutableDiscreteService(IImmutableRepository<FakeImmutableEntity> repository, ILogger logger = null)
                : base(repository, logger)
            {
            }
        }
    }
}