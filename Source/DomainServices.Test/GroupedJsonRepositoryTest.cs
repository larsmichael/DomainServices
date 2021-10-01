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

    public sealed class GroupedJsonRepositoryTest : IDisposable
    {
        private readonly string _filePath = Path.Combine(Path.GetTempPath(), "__grouped-entities.json");
        private readonly GroupedJsonRepository<FakeGroupedEntity> _repository;

        public GroupedJsonRepositoryTest()
        {
            _repository = new GroupedJsonRepository<FakeGroupedEntity>(_filePath);
        }

        public void Dispose()
        {
            File.Delete(_filePath);
        }

        [Fact]
        public void CreateWithNullFilePathThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new GroupedJsonRepository<FakeGroupedEntity>(null!));
        }

        [Theory, AutoData]
        public void AddExistingThrows(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            Assert.Throws<ArgumentException>(() => _repository.Add(entity));
        }

        [Theory, AutoData]
        public void UpdateNonExistingThrows(FakeGroupedEntity entity)
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Update(entity));
        }

        [Fact]
        public void AddWithNoGroupThrows()
        {
            var entity = new FakeGroupedEntity("My Entity", null);
            Assert.Throws<ArgumentException>(() => _repository.Add(entity));
        }

        [Fact]
        public void ContainsWithInvalidGroupedIdThrows()
        {
            Assert.Throws<ArgumentException>(() => _repository.Contains("InvalidGroupedEntityId"));
        }

        [Fact]
        public void GetWithInvalidGroupedIdThrows()
        {
            Assert.Throws<ArgumentException>(() => _repository.Get("InvalidGroupedEntityId"));
        }

        [Fact]
        public void RemoveWithInvalidGroupedIdThrows()
        {
            Assert.Throws<ArgumentException>(() => _repository.Remove("InvalidGroupedEntityId"));
        }

       [Theory, AutoData]
        public void GetNonExistingReturnsEmpty(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            Assert.False(_repository.Get("NonExistingGroup/NonExistingName").HasValue);
        }

        [Theory, AutoData]
        public void GetNonExistingFromExistingGroupReturnsEmpty(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            var id = $"{entity.Group}/NonExistingName";
            Assert.False(_repository.Get(id).HasValue);
        }

        [Theory, AutoData]
        public void AddAndGetIsOk(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            var actual = _repository.Get(entity.FullName).Value;
            Assert.Equal(entity.Id, actual.Id);
        }

        [Theory, AutoData]
        public void ContainsIsOk(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            Assert.True(_repository.Contains(entity.FullName));
        }

        [Theory, AutoData]
        public void DoesNotContainIsOk(FakeGroupedEntity entity)
        {
            Assert.False(_repository.Contains(entity.FullName));
        }

        [Theory, AutoData]
        public void ContainsGroupIsOk(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            Assert.NotNull(entity.Group);
            Assert.True(_repository.ContainsGroup(entity.Group));
        }

        [Theory, AutoData]
        public void DoesNotContainGroupIsOk(FakeGroupedEntity entity)
        {
            Assert.NotNull(entity.Group);
            Assert.False(_repository.ContainsGroup(entity.Group));
        }

        [Theory, AutoData]
        public void CountIsOk(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            Assert.Equal(1, _repository.Count());
        }

        [Theory, AutoData]
        public void GetAllIsOk(FakeGroupedEntity entity1, FakeGroupedEntity entity2)
        {
            _repository.Add(entity1);
            _repository.Add(entity2);
            var entity3 = new FakeGroupedEntity("My Entity", entity1.Group);
            _repository.Add(entity3);
            Assert.Equal(3, _repository.GetAll().Count());
        }

        [Theory, AutoData]
        public void GetByGroupIsOk(FakeGroupedEntity entity1, FakeGroupedEntity entity2)
        {
            _repository.Add(entity1);
            _repository.Add(entity2);
            var entity3 = new FakeGroupedEntity("My Entity", entity1.Group);
            _repository.Add(entity3);
            Assert.NotNull(entity1.Group);
            Assert.NotNull(entity2.Group);
            Assert.Equal(2, _repository.GetByGroup(entity1.Group).Count());
            Assert.Single((IEnumerable) _repository.GetByGroup(entity2.Group));
        }

        [Theory, AutoData]
        public void GetFullNamesByGroupIsOk(FakeGroupedEntity entity1, FakeGroupedEntity entity2)
        {
            _repository.Add(entity1);
            _repository.Add(entity2);
            var entity3 = new FakeGroupedEntity("My Entity", entity1.Group);
            _repository.Add(entity3);
            Assert.NotNull(entity1.Group);
            Assert.NotNull(entity2.Group);
            Assert.Equal(2, _repository.GetFullNames(entity1.Group).Count());
            Assert.Single((IEnumerable) _repository.GetFullNames(entity2.Group));
            Assert.Equal(entity2.FullName, _repository.GetFullNames(entity2.Group).First());
        }

        [Theory, AutoData]
        public void GetFullNamesIsOk(FakeGroupedEntity entity1, FakeGroupedEntity entity2)
        {
            _repository.Add(entity1);
            _repository.Add(entity2);
            var entity3 = new FakeGroupedEntity("My Entity", entity1.Group);
            _repository.Add(entity3);
            Assert.Equal(3, _repository.GetFullNames().Count());
        }

        [Theory, AutoData]
        public void GetIdsIsOk(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            Assert.Equal(entity.Id, _repository.GetIds().First());
        }

        [Theory, AutoData]
        public void RemoveIsOk(FakeGroupedEntity entity1, FakeGroupedEntity entity2)
        {
            _repository.Add(entity1);
            _repository.Add(entity2);
            _repository.Remove(entity1.FullName);
            Assert.False(_repository.Contains(entity1.FullName));
            Assert.True(_repository.Contains(entity2.FullName));
            Assert.Equal(1, _repository.Count());
        }

        [Theory, AutoData]
        public void RemoveUsingPredicateIsOk(FakeGroupedEntity entity1, FakeGroupedEntity entity2)
        {
            _repository.Add(entity1);
            _repository.Add(entity2);
            _repository.Remove(e => e.Id == entity1.Id);
            Assert.False(_repository.Contains(entity1.FullName));
            Assert.Equal(1, _repository.Count());
        }

        [Theory, AutoData]
        public void UpdateIsOk(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            entity.Metadata.Add("Description", "New description");
            _repository.Update(entity);
            Assert.Equal("New description", _repository.Get(entity.FullName).Value.Metadata["Description"]);
        }

        [Theory, AutoData]
        public void UpdateDoesNotModifyAddedDateTime(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            var addedDateTime = _repository.Get(entity.FullName).Value.Added;
            var updatedEntity = new FakeGroupedEntity(entity.Name, entity.Group);
            _repository.Update(updatedEntity);
            Assert.Equal(addedDateTime, _repository.Get(updatedEntity.FullName).Value.Added);
        }

        [Fact]
        public void CaseInsensitiveComparerIsOk()
        {
            var repository = new GroupedJsonRepository<FakeGroupedEntity>(_filePath, comparer: StringComparer.InvariantCultureIgnoreCase);
            repository.Add(new FakeGroupedEntity("MyEntity", "MyGroup"));
            Assert.True(repository.Get("mygroup/myentity").HasValue);
            Assert.True(repository.Contains("mygroup/myentity"));
            Assert.Empty(repository.Get(entity => entity.Name.StartsWith("my")));
            Assert.Single((IEnumerable) repository.Get(entity => entity.Name.StartsWith("my", StringComparison.InvariantCultureIgnoreCase)));
        }

        [Theory, AutoData]
        public void GetAllReturnsClones(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            foreach (var e in _repository.GetAll())
            {
                e.Metadata.Add("Description", "A description");
            }

            Assert.Empty(_repository.Get(entity.FullName).Value.Metadata);
        }

        [Fact]
        public void GetAllForEmptyRepositoryIsOk()
        {
            Assert.Empty(_repository.GetAll());
        }

        [Theory, AutoData]
        public void GetReturnsClone(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            var e = _repository.Get(entity.FullName).Value;
            e.Metadata.Add("Description", "A description");

            Assert.Empty(_repository.Get(entity.FullName).Value.Metadata);
        }

        [Theory, AutoData]
        public void GetByPredicateReturnsClones(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            var e = _repository.Get(ent => ent.Id == entity.Id).First();
            e.Metadata.Add("Description", "A description");

            Assert.Empty(_repository.Get(entity.FullName).Value.Metadata);
        }

        [Theory, AutoData]
        public void GetByGroupReturnsClones(FakeGroupedEntity entity)
        {
            _repository.Add(entity);
            Assert.NotNull(entity.Group);
            var e = _repository.GetByGroup(entity.Group).First();
            e.Metadata.Add("Description", "A description");

            Assert.Empty(_repository.Get(entity.FullName).Value.Metadata);
        }
    }
}