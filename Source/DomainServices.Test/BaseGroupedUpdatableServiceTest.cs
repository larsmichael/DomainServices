namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using AutoFixture.Xunit2;
    using Abstractions;
    using DomainServices.Logging;
    using Repositories;
    using Xunit;

    public class BaseGroupedUpdatableServiceTest : IClassFixture<RepositoryFixture>
    {
        private readonly GroupedUpdatableService _service;
        private readonly int _repeatCount;

        public BaseGroupedUpdatableServiceTest(RepositoryFixture fixture)
        {
            var repository = fixture.Repository;
            _service = new GroupedUpdatableService(repository);
            _repeatCount = fixture.RepeatCount;
        }

        [Fact]
        public void CreateWithNullRepositoryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new GroupedUpdatableService(null));
        }

        [Fact]
        public void GetNonExistingThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => _service.Get("UnknownEntity"));
        }

        [Fact]
        public void GetByGroupForNonExistingThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => _service.GetByGroup("NonExistingGroup"));
        }

        [Fact]
        public void GetByGroupForNullGroupThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _service.GetByGroup(null!));
        }

        [Fact]
        public void GetByGroupForEmptyGroupThrows()
        {
            Assert.Throws<ArgumentException>(() => _service.GetByGroup(""));
        }

        [Fact]
        public void GetFullNamesForNonExistingGroupThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => _service.GetFullNames("NonExistingGroup"));
        }

        [Fact]
        public void GetFullNamesForNullOrEmptyGroupThrows()
        {
            Assert.Throws<ArgumentNullException>(() => _service.GetFullNames(null!, ClaimsPrincipal.Current));
            Assert.Throws<ArgumentException>(() => _service.GetFullNames(""));
        }

        [Theory, AutoFakeEntityData]
        public void UpdateNonExistingThrows(GroupedUpdatableService service, FakeEntity entity)
        {
            Assert.Throws<KeyNotFoundException>(() => service.Update(entity));
        }

        [Theory, AutoFakeEntityData]
        public void RemoveNonExistingThrows(GroupedUpdatableService service, FakeEntity entity)
        {
            Assert.Throws<KeyNotFoundException>(() => service.Remove(entity.Id));
        }

        [Theory, AutoFakeEntityData]
        public void RemoveByNonExistingGroupThrows(GroupedUpdatableService service, string group)
        {
            Assert.Throws<KeyNotFoundException>(() => service.RemoveByGroup(group));
        }

        [Theory, AutoData]
        public void GetIsOk(FakeEntity entity)
        {
            var repository = new FakeGroupedRepository<FakeEntity, string>();
            repository.Add(entity);
            var service = new GroupedUpdatableService(repository);
            Assert.Equal(entity.Id, service.Get(entity.Id).Id);
        }

        [Theory, AutoData]
        public void GetManyIsOk(FakeEntity[] entities)
        {
            var repository = new FakeGroupedRepository<FakeEntity, string>();
            foreach (var entity in entities)
            {
                repository.Add(entity);
            }

            var service = new GroupedUpdatableService(repository);
            var myEntities = service.Get(entities.Select(e => e.Id)).ToArray();
            Assert.Equal(_repeatCount, myEntities.Length);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
        }

        [Theory, AutoData]
        public void GetManyWithNonExistingIdIsOk(FakeEntity[] entities)
        {
            var repository = new FakeGroupedRepository<FakeEntity, string>();
            foreach (var entity in entities)
            {
                repository.Add(entity);
            }

            var service = new GroupedUpdatableService(repository);
            var ids = entities.Select(e => e.Id).ToList();
            ids.Add("NonExistingId");
            var myEntities = service.Get(ids).ToArray();
            Assert.Equal(_repeatCount, myEntities.Length);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
            Assert.DoesNotContain("NonExistingId", myEntities.Select(e => e.Id));
        }

        [Theory, AutoData]
        public void GetManyWithNonExistingIdLogsWarning(FakeEntity[] entities)
        {
            var repository = new FakeGroupedRepository<FakeEntity, string>();
            foreach (var entity in entities)
            {
                repository.Add(entity);
            }

            var logger = new FakeLogRepository();
            var service = new GroupedUpdatableService(repository, logger);
            var ids = entities.Select(e => e.Id).ToList();
            ids.Add("NonExistingId");
            var myEntities = service.Get(ids).ToArray();
            Assert.Equal(_repeatCount, myEntities.Length);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
            Assert.DoesNotContain("NonExistingId", myEntities.Select(e => e.Id));
            var query = new Query<LogEntry>(new QueryCondition("LogLevel", LogLevel.Warning));
            var logEntries = logger.Get(query).ToArray();
            Assert.Single(logEntries);
            Assert.Contains("'DomainServices.Test.FakeEntity' with id 'NonExistingId' was not found.", logEntries.Select(l => l.Text));
        }

        [Theory, AutoFakeEntityData]
        public void GetByGroupIsOk(GroupedUpdatableService service, FakeEntity entity)
        {
            var group = entity.Group;
            service.Add(entity);

            Assert.NotNull(group);
            Assert.NotEmpty(group);
            Assert.True(service.GetByGroup(group).Any());
        }

        [Theory, AutoFakeEntityData]
        public void GetByGroupsIsOk(GroupedUpdatableService service, FakeEntity entity)
        {
            var group = entity.Group;
            service.Add(entity);

            Assert.NotNull(group);
            Assert.NotEmpty(group);
            Assert.True(service.GetByGroups(new List<string> { group, group }).Any());
        }

        [Theory, AutoFakeEntityData]
        public void GetFullNamesByGroupIsOk(GroupedUpdatableService service, FakeEntity entity)
        {
            var group = entity.Group;
            service.Add(entity);

            Assert.NotNull(group);
            Assert.NotEmpty(group);
            var fullNames = service.GetFullNames(group).ToList();
            Assert.True(fullNames.Any());

            var fullName = FullName.Parse(fullNames[0]);
            Assert.NotNull(fullName.Group);
            Assert.NotEmpty(fullName.Group);
            Assert.NotEmpty(fullName.Name);
        }

        [Fact]
        public void GetFullNamesIsOk()
        {
            Assert.True(_service.GetFullNames().Any());
        }

        [Theory, AutoFakeEntityData]
        public void AddAndGetIsOk(GroupedUpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            var e = service.Get(entity.Id);
            Assert.Equal(entity.Id, e.Id);
            Assert.Null(e.Updated);
            Assert.NotNull(e.Added);
            Assert.InRange(e.Added.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Theory, AutoFakeEntityData]
        public void UpdateIsOk(GroupedUpdatableService service, FakeEntity entity)
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
        public void AddOrUpdateIsOk(GroupedUpdatableService service, FakeEntity entity)
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
        public void TryAddIsOk(GroupedUpdatableService service, FakeEntity entity)
        {
            Assert.True(service.TryAdd(entity));
            var e = service.Get(entity.Id);
            Assert.Equal(entity.Id, e.Id);
            Assert.Null(e.Updated);
            Assert.NotNull(e.Added);
            Assert.InRange(e.Added.Value, DateTime.UtcNow.AddSeconds(-1), DateTime.UtcNow.AddSeconds(1));
        }

        [Theory, AutoFakeEntityData]
        public void TryAddExistingReturnsFalse(GroupedUpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            Assert.False(service.TryAdd(entity));
        }

        [Theory, AutoFakeEntityData]
        public void TryUpdateIsOk(GroupedUpdatableService service, FakeEntity entity)
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
        public void TryUpdateNonExistingReturnsFalse(GroupedUpdatableService service, FakeEntity entity)
        {
            Assert.False(service.TryUpdate(entity));
        }

        [Theory, AutoFakeEntityData]
        public void RemoveIsOk(GroupedUpdatableService service, FakeEntity entity)
        {
            service.Add(entity);
            service.Remove(entity.Id);

            Assert.Throws<KeyNotFoundException>(() => service.Get(entity.Id));
        }

        [Theory, AutoFakeEntityData]
        public void RemoveByGroupIsOk(GroupedUpdatableService service)
        {
            const string myGroup = "MyGroup";
            const string anotherGroup = "anotherGroup";
            var foo = new FakeEntity(Guid.NewGuid().ToString(), "foo", myGroup);
            var bar = new FakeEntity(Guid.NewGuid().ToString(), "bar", myGroup);
            var baz = new FakeEntity(Guid.NewGuid().ToString(), "baz", anotherGroup);
            service.Add(foo);
            service.Add(bar);
            service.Add(baz);
            service.RemoveByGroup(myGroup);

            Assert.Throws<KeyNotFoundException>(() => service.Get(foo.Id));
            Assert.Throws<KeyNotFoundException>(() => service.Get(bar.Id));
            Assert.Throws<KeyNotFoundException>(() => service.GetByGroup(myGroup));
            Assert.Single(service.GetByGroup(anotherGroup));
        }

        [Theory, AutoFakeEntityData]
        public void EventsAreRaisedOnAdd(GroupedUpdatableService service, FakeEntity entity)
        {
            var raisedEvents = new List<string>();
            service.Adding += (_, _) => { raisedEvents.Add("Adding"); };
            service.Added += (_, _) => { raisedEvents.Add("Added"); };

            service.Add(entity);

            Assert.Equal("Adding", raisedEvents[0]);
            Assert.Equal("Added", raisedEvents[1]);
        }

        [Theory, AutoFakeEntityData]
        public void EventsAreRaisedOnUpdate(GroupedUpdatableService service, FakeEntity entity)
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
        public void EventsAreRaisedOnRemove(GroupedUpdatableService service, FakeEntity entity)
        {
            var raisedEvents = new List<string>();
            service.Deleting += (_, _) => { raisedEvents.Add("Deleting"); };
            service.Deleted += (_, _) => { raisedEvents.Add("Deleted"); };
            service.Add(entity);

            service.Remove(entity.Id);

            Assert.Equal("Deleting", raisedEvents[0]);
            Assert.Equal("Deleted", raisedEvents[1]);
        }

        [Theory, AutoFakeEntityData]
        public void EventsAreRaisedOnRemoveByGroup(GroupedUpdatableService service, FakeEntity entity)
        {
            var raisedEvents = new List<string>();
            service.DeletingGroup += (_, _) => { raisedEvents.Add("DeletingGroup"); };
            service.DeletedGroup += (_, _) => { raisedEvents.Add("DeletedGroup"); };
            service.Add(entity);

            Assert.NotNull(entity.Group);
            service.RemoveByGroup(entity.Group);

            Assert.Equal("DeletingGroup", raisedEvents[0]);
            Assert.Equal("DeletedGroup", raisedEvents[1]);
        }

        public class GroupedUpdatableService : BaseGroupedUpdatableService<FakeEntity, string>
        {
            public GroupedUpdatableService(IGroupedRepository<FakeEntity> repository)
                : base(repository)
            {
            }

            public GroupedUpdatableService(IGroupedRepository<FakeEntity> repository, ILogger logger)
                : base(repository, logger)
            {
            }
        }
    }
}