namespace DomainServices.Test
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Claims;
    using AutoFixture.Xunit2;
    using Repositories;
    using Xunit;

    public sealed class JsonRepositorySecuredTest : IClassFixture<JsonRepositorySecuredFixture>, IDisposable
    {
        private readonly string _filePath = Path.Combine(Path.GetTempPath(), "__secured-entities.json");
        private readonly JsonRepositorySecured<FakeEntity, string> _repository;
        private readonly ClaimsPrincipal _admin;
        private readonly ClaimsPrincipal _user;
        private readonly ClaimsPrincipal _guest;

        public JsonRepositorySecuredTest(JsonRepositorySecuredFixture fixture)
        {
            _repository = new JsonRepositorySecured<FakeEntity, string>(_filePath);
            _admin = fixture.Admin;
            _user = fixture.User;
            _guest = fixture.Guest;
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
        public void AddExistingThrows(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            Assert.Throws<ArgumentException>(() => _repository.Add(entity));
        }

        [Theory, AutoData]
        public void UpdateNonExistingThrows(FakeEntitySecured entity)
        {
            Assert.Throws<KeyNotFoundException>(() => _repository.Update(entity, _user));
        }

        [Theory, AutoData]
        public void AddWithoutPermissionsThrows(FakeEntity entity)
        {
            Assert.Throws<ArgumentException>(() => _repository.Add(entity));
        }

        [Theory, AutoData]
        public void AddWithoutReadPermissionsThrows(FakeEntity entity)
        {
            entity.AddPermissions(new [] {"Partygoers"}, new [] {"dance"});
            Assert.Throws<ArgumentException>(() => _repository.Add(entity));
        }

        [Theory, AutoData]
        public void UpdateWithoutPermissionsThrows(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            entity.Permissions.Clear();
            Assert.Throws<ArgumentException>(() => _repository.Update(entity, _user));
        }

        [Theory, AutoData]
        public void UpdateWithoutReadPermissionsThrows(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            entity.Permissions.Clear();
            entity.AddPermissions(new[] { "Partygoers" }, new[] { "dance" });
            Assert.Throws<ArgumentException>(() => _repository.Update(entity, _user));
        }

        [Theory, AutoData]
        public void UpdateWithoutPermissionThrows(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            entity.Metadata.Add("Description", "New description");
            Assert.Throws<ArgumentException>(() => _repository.Update(entity, _user));
        }

        [Theory, AutoData]
        public void RemoveWithoutPermissionThrows(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            Assert.Throws<ArgumentException>(() => _repository.Remove(entity.Id, _user));
        }

        [Theory, AutoData]
        public void AddAndGetIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            var actual = _repository.Get(entity.Id, _user).Value;
            Assert.Equal(entity.Id, actual.Id);
        }

        [Theory, AutoData]
        public void ContainsIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            Assert.True(_repository.Contains(entity.Id, _user));
            Assert.False(_repository.Contains(entity.Id, _guest));
        }

        [Theory, AutoData]
        public void DoesNotContainIsOk(string id)
        {
            Assert.False(_repository.Contains(id, _user));
        }

        [Theory, AutoData]
        public void CountIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            Assert.Equal(1, _repository.Count(_user));
            Assert.Equal(0, _repository.Count(_guest));
        }

        [Theory, AutoData]
        public void GetAllIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            Assert.Single((IEnumerable) _repository.GetAll(_user));
            Assert.Empty(_repository.GetAll(_guest));
        }

        [Theory, AutoData]
        public void GetIdsIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            Assert.Equal(entity.Id, _repository.GetIds(_user).First());
            Assert.Empty(_repository.GetIds(_guest));
        }

        [Theory, AutoData]
        public void RemoveIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            _repository.Remove(entity.Id, _admin);
            Assert.False(_repository.Contains(entity.Id, _user));
            Assert.Equal(0, _repository.Count(_user));
        }

        [Theory, AutoData]
        public void RemoveUsingPredicateIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            _repository.Remove(e => e.Id == entity.Id, _admin);
            Assert.False(_repository.Contains(entity.Id, _user));
            Assert.Equal(0, _repository.Count(_user));
        }

        [Theory, AutoData]
        public void UpdateIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            entity.Metadata.Add("Description", "New description");
            _repository.Update(entity, _admin);
            Assert.Equal("New description", _repository.Get(entity.Id, _user).Value.Metadata["Description"]);
        }

        [Theory, AutoData]
        public void UpdateDoesNotModifyAddedDateTime(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            var addedDateTime = _repository.Get(entity.Id, _user).Value.Added;
            var updatedEntity = new FakeEntitySecured(entity.Id, "New Name");
            _repository.Update(updatedEntity, _admin);
            Assert.Equal(addedDateTime, _repository.Get(updatedEntity.Id, _user).Value.Added);
        }

        [Fact]
        public void CaseInsensitiveComparerIsOk()
        {
            var repository = new JsonRepositorySecured<FakeEntity, string>(_filePath, comparer: StringComparer.InvariantCultureIgnoreCase);
            repository.Add(new FakeEntitySecured("MyEntity", "My Entity"));
            Assert.True(repository.Get("myentity", _user).HasValue);
            Assert.True(repository.Contains("myentity", _user));
            Assert.Empty(repository.Get(entity => entity.Id.StartsWith("my"), _user));
            Assert.Single((IEnumerable) repository.Get(entity => entity.Id.StartsWith("my", StringComparison.InvariantCultureIgnoreCase), _user));
        }

        [Theory, AutoData]
        public void GetAllReturnsClones(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            foreach (var e in _repository.GetAll(_user))
            {
                e.Metadata.Add("Description", "A description");
            }

            Assert.Empty(_repository.Get(entity.Id, _user).Value.Metadata);
        }

        [Fact]
        public void GetAllForEmptyRepositoryIsOk()
        {
            Assert.Empty(_repository.GetAll(_user));
        }

        [Theory, AutoData]
        public void GetReturnsClone(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            var e = _repository.Get(entity.Id, _user).Value;
            e.Metadata.Add("Description", "A description");

            Assert.Empty(_repository.Get(entity.Id, _user).Value.Metadata);
        }

        [Theory, AutoData]
        public void GetByPredicateReturnsClones(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            var e = _repository.Get(ent => ent.Id == entity.Id, _user).First();
            e.Metadata.Add("Description", "A description");

            Assert.Empty(_repository.Get(entity.Id, _user).Value.Metadata);
        }

        [Theory, AutoData]
        public void GetByPredicateIsOk(FakeEntitySecured entity)
        {
            _repository.Add(entity);
            Assert.Single((IEnumerable) _repository.Get(ent => ent.Id == entity.Id, _user));
            Assert.Empty(_repository.Get(ent => ent.Id == entity.Id, _guest));
        }
    }
}