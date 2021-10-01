namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Abstractions;
    using DomainServices.Logging;
    using Repositories;
    using Xunit;

    public class BaseUpdatableServiceTest
    {
        [Fact]
        public void CreateWithNullRepositoryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new UpdatableService(null));
        }

        [Theory, AutoFakeEntityData]
        public void GetNonExistingThrows(UpdatableService service)
        {
            Assert.Throws<KeyNotFoundException>(() => service.Get("UnknownEntity"));
        }

        [Theory, AutoFakeEntityData]
        public void UpdateNonExistingThrows(UpdatableService service, FakeEntity entity)
        {
            Assert.Throws<KeyNotFoundException>(() => service.Update(entity));
        }

        [Theory, AutoFakeEntityData]
        public void RemoveNonExistingThrows(UpdatableService service, FakeEntity entity)
        {
            Assert.Throws<KeyNotFoundException>(() => service.Remove(entity.Id));
        }

        [Theory, AutoFakeEntityData]
        public void AddAndGetIsOk(UpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            var e = service.Get(entity.Id);
            Assert.Equal(entity.Id, e.Id);
            Assert.Null(e.Updated);
            Assert.NotNull(e.Added);
            Assert.InRange(e.Added.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Theory, AutoFakeEntityData]
        public void GetManyIsOk(UpdatableService service, FakeEntity[] entities)
        {
            foreach (var entity in entities)
            {
                service.Add(entity);
            }

            var myEntities = service.Get(entities.Select(e => e.Id)).ToArray();
            Assert.NotEmpty(myEntities);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
        }

        [Theory, AutoFakeEntityData]
        public void GetManyWithNonExistingIdIsOk(UpdatableService service, FakeEntity[] entities)
        {
            foreach (var entity in entities)
            {
                service.Add(entity);
            }

            var ids = entities.Select(e => e.Id).ToList();
            ids.Add("NonExistingId");
            var myEntities = service.Get(ids).ToArray();
            Assert.NotEmpty(myEntities);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
            Assert.DoesNotContain("NonExistingId", myEntities.Select(e => e.Id));
        }

        [Theory, AutoFakeEntityData]
        public void GetManyWithNonExistingIdLogsWarning(FakeEntity[] entities)
        {
            var repository = new FakeGroupedRepository<FakeEntity, string>();
            foreach (var entity in entities)
            {
                repository.Add(entity);
            }

            var logger = new FakeLogRepository();
            var service = new UpdatableService(repository, logger);
            var ids = entities.Select(e => e.Id).ToList();
            ids.Add("NonExistingId");
            var myEntities = service.Get(ids).ToArray();
            Assert.NotEmpty(myEntities);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
            Assert.DoesNotContain("NonExistingId", myEntities.Select(e => e.Id));
            var query = new Query<LogEntry>(new QueryCondition("LogLevel", LogLevel.Warning));
            var logEntries = logger.Get(query).ToArray();
            Assert.Single(logEntries);
            Assert.Contains("'DomainServices.Test.FakeEntity' with id 'NonExistingId' was not found.", logEntries.Select(l => l.Text));
        }

        [Theory, AutoFakeEntityData]
        public void UpdateIsOk(UpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            var updatedEntity = new FakeEntity(entity.Id, "Updated name");
            service.Update(updatedEntity);
            var e = service.Get(entity.Id);

            Assert.Equal(updatedEntity.Name, e.Name);
            Assert.NotNull(e.Updated);
            Assert.InRange(e.Updated.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Theory, AutoFakeEntityData]
        public void AddOrUpdateIsOk(UpdatableService service, FakeEntity entity)
        {
            var raisedEvents = new List<string>();
            service.Added += (_, _) => { raisedEvents.Add("Added"); };
            service.Updated += (_, _) => { raisedEvents.Add("Updated"); };
            service.AddOrUpdate(entity);
            var updated = new FakeEntity(entity.Id, "Updated name");
            service.AddOrUpdate(updated);
            var e = service.Get(entity.Id);

            Assert.Equal("Added", raisedEvents[0]);
            Assert.Equal("Updated", raisedEvents[1]);
            Assert.Equal(updated.Name, e.Name);
            Assert.NotNull(e.Updated);
            Assert.InRange(e.Updated.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Theory, AutoFakeEntityData]
        public void TryAddIsOk(UpdatableService service, FakeEntity entity)
        {
            Assert.True(service.TryAdd(entity));
            var e = service.Get(entity.Id);
            Assert.Equal(entity.Id, e.Id);
            Assert.Null(e.Updated);
            Assert.NotNull(e.Added);
            Assert.InRange(e.Added.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Theory, AutoFakeEntityData]
        public void TryAddExistingReturnsFalse(UpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            Assert.False(service.TryAdd(entity));
        }

        [Theory, AutoFakeEntityData]
        public void TryUpdateIsOk(UpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            var updatedEntity = new FakeEntity(entity.Id, "Updated name");

            Assert.True(service.TryUpdate(updatedEntity));
            var e = service.Get(entity.Id);
            Assert.Equal(updatedEntity.Name, e.Name);
            Assert.NotNull(e.Added);
            Assert.NotNull(e.Updated);
            Assert.InRange(e.Updated.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Theory, AutoFakeEntityData]
        public void TryUpdateNonExistingReturnsFalse(UpdatableService service, FakeEntity entity)
        {
            Assert.False(service.TryUpdate(entity));
        }

        [Theory, AutoFakeEntityData]
        public void RemoveIsOk(UpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            service.Remove(entity.Id);

            Assert.Throws<KeyNotFoundException>(() => service.Get(entity.Id));
        }

        [Theory, AutoFakeEntityData]
        public void EventsAreRaisedOnAdd(UpdatableService service, FakeEntity entity)
        {
            var raisedEvents = new List<string>();
            service.Adding += (_, _) => { raisedEvents.Add("Adding"); };
            service.Added += (_, _) => { raisedEvents.Add("Added"); };

            service.Add(entity);

            Assert.Equal("Adding", raisedEvents[0]);
            Assert.Equal("Added", raisedEvents[1]);
        }

        [Theory, AutoFakeEntityData]
        public void EventsAreRaisedOnUpdate(UpdatableService service, FakeEntity entity)
        {
            var raisedEvents = new List<string>();
            service.Updating += (_, _) => { raisedEvents.Add("Updating"); };
            service.Updated += (_, _) => { raisedEvents.Add("Updated"); };
            service.Add(entity);

            var updatedAccount = new FakeEntity(entity.Id, "Updated name");
            service.Update(updatedAccount);

            Assert.Equal("Updating", raisedEvents[0]);
            Assert.Equal("Updated", raisedEvents[1]);
        }

        [Theory, AutoFakeEntityData]
        public void EventsAreRaisedOnRemove(UpdatableService service, FakeEntity entity)
        {
            var raisedEvents = new List<string>();
            service.Deleting += (_, _) => { raisedEvents.Add("Deleting"); };
            service.Deleted += (_, _) => { raisedEvents.Add("Deleted"); };
            service.Add(entity);

            service.Remove(entity.Id);

            Assert.Equal("Deleting", raisedEvents[0]);
            Assert.Equal("Deleted", raisedEvents[1]);
        }

        public class UpdatableService : BaseUpdatableService<FakeEntity, string>
        {
            public UpdatableService(IUpdatableRepository<FakeEntity, string> repository)
                : base(repository)
            {
            }

            public UpdatableService(IUpdatableRepository<FakeEntity, string> repository, ILogger logger)
                : base(repository, logger)
            {
            }
        }
    }
}