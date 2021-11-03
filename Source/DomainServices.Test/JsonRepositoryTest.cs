namespace DomainServices.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoFixture.Xunit2;
    using Repositories;
    using Xunit;

    public sealed class JsonRepositoryTest : IDisposable
    {
        private readonly string _filePath = Path.Combine(Path.GetTempPath(), "__entities.json");
        private readonly JsonRepository<FakeEntity, string> _repository;

        public JsonRepositoryTest()
        {
            _repository = new JsonRepository<FakeEntity, string>(_filePath);
        }

        public void Dispose()
        {
            File.Delete(_filePath);
        }

        [Fact]
        public void CreateWithNullFilePathThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new JsonRepository<FakeEntity, string>(null!));
        }

        [Theory, AutoData]
        public void AddExistingThrows(FakeEntity entity)
        {
            _repository.Add(entity);
            Assert.Throws<ArgumentException>(() => _repository.Add(entity));
        }

        [Theory, AutoData]
        public void UpdateNonExistingThrows(FakeEntity entity)
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Update(entity));
        }

        [Theory, AutoData]
        public void AddAndGetIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            var actual = _repository.Get(entity.Id).Value;
            Assert.Equal(entity.Id, actual.Id);
        }

        [Theory, AutoData]
        public void ContainsIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            Assert.True(_repository.Contains(entity.Id));
        }

        [Theory, AutoData]
        public void DoesNotContainIsOk(string id)
        {
            Assert.False(_repository.Contains(id));
        }

        [Theory, AutoData]
        public void CountIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            Assert.Equal(1, _repository.Count());
        }

        [Theory, AutoData]
        public void GetAllIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            Assert.Single((IEnumerable) _repository.GetAll());
        }

        [Theory, AutoData]
        public void GetIdsIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            Assert.Equal(entity.Id, _repository.GetIds().First());
        }

        [Theory, AutoData]
        public void RemoveIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            _repository.Remove(entity.Id);
            Assert.False(_repository.Contains(entity.Id));
            Assert.Equal(0, _repository.Count());
        }

        [Theory, AutoData]
        public void RemoveUsingPredicateIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            _repository.Remove(e => e.Id == entity.Id);
            Assert.False(_repository.Contains(entity.Id));
            Assert.Equal(0, _repository.Count());
        }

        [Theory, AutoData]
        public void UpdateIsOk(FakeEntity entity)
        {
            _repository.Add(entity);
            entity.Metadata.Add("Description", "New description");
            _repository.Update(entity);
            var updatedEntity = _repository.Get(entity.Id).Value;
            Assert.NotEmpty(updatedEntity.Metadata);
            Assert.Equal("New description", updatedEntity.Metadata["Description"]);
        }

        [Theory, AutoData]
        public void UpdateDoesNotModifyAddedDateTime(FakeEntity entity)
        {
            _repository.Add(entity);
            var addedDateTime = _repository.Get(entity.Id).Value.Added;
            var updatedEntity = new FakeEntity(entity.Id, "New Name");
            _repository.Update(updatedEntity);
            Assert.Equal(addedDateTime, _repository.Get(updatedEntity.Id).Value.Added);
        }

        [Fact]
        public void CaseInsensitiveComparerIsOk()
        {
            var repository = new JsonRepository<FakeEntity, string>(_filePath, comparer: StringComparer.InvariantCultureIgnoreCase);
            repository.Add(new FakeEntity("MyEntity", "My Entity"));
            Assert.True(repository.Get("myentity").HasValue);
            Assert.True(repository.Contains("myentity"));
            Assert.Empty(repository.Get(entity => entity.Id.StartsWith("my")));
            Assert.Single((IEnumerable) repository.Get(entity => entity.Id.StartsWith("my", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Theory, AutoData]
        public void GetAllReturnsClones(FakeEntity entity)
        {
            _repository.Add(entity);
            foreach (var e in _repository.GetAll())
            {
                e.Metadata.Add("Description", "A description");
            }

            Assert.DoesNotContain("Description", _repository.Get(entity.Id).Value.Metadata.Keys);
        }

        [Fact]
        public void GetAllForEmptyRepositoryIsOk()
        {
            Assert.Empty(_repository.GetAll());
        }

        [Theory, AutoData]
        public void GetReturnsClone(FakeEntity entity)
        {
            _repository.Add(entity);
            var e = _repository.Get(entity.Id).Value;
            e.Metadata.Add("Description", "A description");

            Assert.DoesNotContain("Description", _repository.Get(entity.Id).Value.Metadata.Keys);
        }

        [Theory, AutoData]
        public void GetByPredicateReturnsClones(FakeEntity entity)
        {
            _repository.Add(entity);
            var e = _repository.Get(ent => ent.Id == entity.Id).First();
            e.Metadata.Add("Description", "A description");

            Assert.DoesNotContain("Description", _repository.Get(entity.Id).Value.Metadata.Keys);
        }
    }
}