namespace DomainServices.Test
{
    using System;
    using System.IO;
    using System.Linq;
    using AutoFixture.Xunit2;
    using Repositories;
    using Xunit;

    public sealed class ImmutableJsonRepositoryTest : IDisposable
    {
        private readonly string _filePath = Path.Combine(Path.GetTempPath(), "__immutable-entities.json");
        private readonly ImmutableJsonRepository<FakeEntity, string> _repository;

        public ImmutableJsonRepositoryTest()
        {
            _repository = new ImmutableJsonRepository<FakeEntity, string>(_filePath);
        }

        public void Dispose()
        {
            File.Delete(_filePath);
        }

        [Fact]
        public void CreateWithNullFilePathThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new ImmutableJsonRepository<FakeEntity, string>(null!));
        }

        [Theory, AutoData]
        public void AddExistingThrows(FakeEntity entity)
        {
            _repository.Add(entity);
            Assert.Throws<ArgumentException>(() => _repository.Add(entity));
        }

        [Theory, AutoData]
        public void AddAndGetIsOk(FakeEntity entity)
        {
            entity.Metadata.Add("foo", true);
            _repository.Add(entity);
            var actual = _repository.Get(entity.Id).Value;
            Assert.Equal(entity.Id, actual.Id);
            Assert.Contains("foo", actual.Metadata);
            Assert.IsType<bool>(actual.Metadata["foo"]);
            Assert.True((bool)actual.Metadata["foo"]);
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
            Assert.Single(_repository.GetAll());
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

        [Fact]
        public void CaseInsensitiveComparerIsOk()
        {
            var repository = new ImmutableJsonRepository<FakeEntity, string>(_filePath, comparer: StringComparer.InvariantCultureIgnoreCase);
            repository.Add(new FakeEntity("MyEntity", "My Entity"));
            Assert.True(repository.Get("myentity").HasValue);
            Assert.True(repository.Contains("myentity"));
            Assert.Empty(repository.Get(entity => entity.Id.StartsWith("my")));
            Assert.Single(repository.Get(entity => entity.Id.StartsWith("my", StringComparison.InvariantCultureIgnoreCase)));
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