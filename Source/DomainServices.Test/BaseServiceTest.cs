namespace DomainServices.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture.Xunit2;
    using Abstractions;
    using DomainServices.Logging;
    using Repositories;
    using Xunit;

    public class BaseServiceTest : IClassFixture<RepositoryFixture>
    {
        private readonly Service _service;
        private readonly int _repeatCount;

        public BaseServiceTest(RepositoryFixture fixture)
        {
            var repository = fixture.Repository;
            _service = new Service(repository);
            _repeatCount = fixture.RepeatCount;
        }

        [Fact]
        public void CreateWithNullRepositoryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Service(null));
        }

        [Fact]
        public void GetNonExistingThrows()
        {
            Assert.Throws<KeyNotFoundException>(() => _service.Get("UnknownEntity"));
        }

        [Theory, AutoData]
        public void GetIsOk(FakeEntity entity)
        {
            var repository = new FakeRepository<FakeEntity, string>();
            repository.Add(entity);
            var service = new Service(repository);
            Assert.Equal(entity.Id, service.Get(entity.Id).Id);
        }

        [Theory, AutoData]
        public void GetManyIsOk(FakeEntity[] entities)
        {
            var repository = new FakeRepository<FakeEntity, string>();
            foreach (var entity in entities)
            {
                repository.Add(entity);
            }
            
            var service = new Service(repository);
            var myEntities = service.Get(entities.Select(e => e.Id)).ToArray();
            Assert.Equal(_repeatCount, myEntities.Length);
            Assert.Contains(entities[0].Id, myEntities.Select(e => e.Id));
        }

        [Theory, AutoData]
        public void GetManyWithNonExistingIdIsOk(FakeEntity[] entities)
        {
            var repository = new FakeRepository<FakeEntity, string>();
            foreach (var entity in entities)
            {
                repository.Add(entity);
            }

            var service = new Service(repository);
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
            var repository = new FakeRepository<FakeEntity, string>();
            foreach (var entity in entities)
            {
                repository.Add(entity);
            }

            var logger = new FakeLogRepository();
            var service = new Service(repository, logger);
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

        private class Service : BaseService<FakeEntity, string>
        {
            public Service(IRepository<FakeEntity, string> repository, ILogger logger = null)
                : base(repository, logger)
            {
            }
        }
    }
}