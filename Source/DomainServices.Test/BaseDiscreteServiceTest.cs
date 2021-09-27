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

    public class BaseDiscreteServiceTest : IClassFixture<RepositoryFixture>
    {
        private readonly DiscreteService _service;
        private readonly int _repeatCount;

        public BaseDiscreteServiceTest(RepositoryFixture fixture)
        {
            var repository = fixture.Repository;
            _service = new DiscreteService(repository);
            _repeatCount = fixture.RepeatCount;
        }

        [Fact]
        public void CreateWithNullRepositoryThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new DiscreteService(null));
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
            var service = new DiscreteService(repository);
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

            var service = new DiscreteService(repository);
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

            var service = new DiscreteService(repository);
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
            var service = new DiscreteService(repository, logger);
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

        private class DiscreteService : BaseDiscreteService<FakeEntity, string>
        {
            public DiscreteService(IDiscreteRepository<FakeEntity, string> repository, ILogger logger = null)
                : base(repository, logger)
            {
            }
        }
    }
}