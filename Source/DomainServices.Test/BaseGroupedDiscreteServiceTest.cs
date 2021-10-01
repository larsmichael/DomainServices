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

    public class BaseGroupedDiscreteServiceTest : IClassFixture<RepositoryFixture>
    {
        private readonly GroupedDiscreteService _service;
        private readonly int _repeatCount;

        public BaseGroupedDiscreteServiceTest(RepositoryFixture fixture)
        {
            var repository = fixture.Repository;
            _service = new GroupedDiscreteService(repository);
            _repeatCount = fixture.RepeatCount;
        }

        [Fact]
        public void CreateWithNullRepositoryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new GroupedDiscreteService(null));
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
            Assert.Throws<ArgumentException>(() => _service.GetFullNames(""));
            Assert.Throws<ArgumentNullException>(() => _service.GetFullNames(null!, ClaimsPrincipal.Current));
        }

        [Theory, AutoData]
        public void GetIsOk(FakeEntity entity)
        {
            var repository = new FakeGroupedRepository<FakeEntity, string>();
            repository.Add(entity);
            var service = new GroupedDiscreteService(repository);
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

            var service = new GroupedDiscreteService(repository);
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

            var service = new GroupedDiscreteService(repository);
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
            var service = new GroupedDiscreteService(repository, logger);
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

        [Fact]
        public void GetByGroupIsOk()
        {
            var group = _service.GetAll().ToArray()[0].Group;
            Assert.NotNull(group);
            Assert.NotEmpty(group);
            Assert.True(_service.GetByGroup(group).Any());
        }

        [Fact]
        public void GetByGroupsIsOk()
        {
            var group = _service.GetAll().ToArray()[0].Group;
            Assert.NotNull(group);
            Assert.NotEmpty(group);
            Assert.True(_service.GetByGroups(new List<string> { group, group }).Any());
        }

        [Fact]
        public void GetFullNamesByGroupIsOk()
        {
            var group = _service.GetAll().ToArray()[0].Group;
            Assert.NotNull(group);
            Assert.NotEmpty(group);
            var fullNames = _service.GetFullNames(group).ToList();
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
            Assert.False(_service.Exists("NonExistingEntity"));
        }

        private class GroupedDiscreteService : BaseGroupedDiscreteService<FakeEntity, string>
        {
            public GroupedDiscreteService(IGroupedRepository<FakeEntity> repository)
                : base(repository)
            {
            }

            public GroupedDiscreteService(IGroupedRepository<FakeEntity> repository, ILogger logger)
                : base(repository, logger)
            {
            }
        }
    }
}