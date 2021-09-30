namespace DomainServices.Test
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using DomainServices.Authorization;
    using Xunit;

    public class BaseEntityTest
    {
        [Fact]
        public void AddPermissionIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermission(new [] {"Administrators", "John.Doe"}, "read");
            Assert.Single(entity.Permissions);
            Assert.Equal(2, entity.Permissions[0].Principals.Count);
        }

        [Fact]
        public void AddPermissionWithDuplicatePrincipalsIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermission(new[] { "Administrators", "Administrators" }, "read");
            Assert.Single(entity.Permissions[0].Principals);
        }

        [Fact]
        public void AddMultiplePermissionsIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermissions(new[] { "Administrators", "John.Doe" }, new [] {"read", "write"});
            Assert.Equal(2, entity.Permissions.Count);
        }

        [Fact]
        public void RemovePermissionIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermission(new[] { "Administrators", "John.Doe" }, "read");
            Assert.Single(entity.Permissions);
        }

        [Fact]
        public void IsAllowedIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermission(new [] { "Administrators", "John.Doe" }, "read");
            entity.AddPermission(new [] { "Administrators" }, "write");

            Assert.True(entity.IsAllowed(new [] {"John.Doe"}, "read"));
            Assert.True(entity.IsAllowed(new[] { "Administrators" }, "read"));
            Assert.False(entity.IsAllowed(new[] { "John.Doe" }, "write"));
            Assert.True(entity.IsAllowed(new[] { "John.Doe", "Administrators" }, "write"));
        }

        [Fact]
        public void IsDeniedIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermission(new[] { "Administrators", "John.Doe" }, "read");
            entity.AddPermission(new[] { "John.Doe" }, "read", PermissionType.Denied);

            Assert.False(entity.IsAllowed(new[] { "John.Doe" }, "read"));
            Assert.False(entity.IsAllowed(new[] { "John.Doe", "Administrators" }, "read"));
        }

        [Fact]
        public void IsAllowedByUserIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermission(new[] { "Administrators", "John.Doe" }, "read");
            entity.AddPermission(new[] { "Administrators" }, "write");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "John.Doe"),
                new Claim(ClaimTypes.GroupSid, "Administrators"),

            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);

            Assert.True(entity.IsAllowed(user, "read"));
            Assert.True(entity.IsAllowed(user, "write"));
            Assert.False(entity.IsAllowed(user, "delete"));
        }

        [Fact]
        public void IsDeniedByUserIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            entity.AddPermission(new[] { "Administrators", "John.Doe" }, "read");
            entity.AddPermission(new[] { "John.Doe" }, "read", PermissionType.Denied);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "John Doe"),
                new Claim(ClaimTypes.NameIdentifier, "John.Doe"),
                new Claim(ClaimTypes.GroupSid, "Administrators"),

            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var user = new ClaimsPrincipal(identity);

            Assert.False(entity.IsAllowed(user, "read"));
        }

        [Fact]
        public void CloneIsOk()
        {
            var entity = new FakeEntity("MyEntity", "My Entity");
            var clone = entity.Clone<FakeEntity>();
            Assert.Equal(entity.Id, clone.Id);
            Assert.Equal(entity.Name, clone.Name);
            Assert.Equal(entity.FullName, clone.FullName);
        }
    }
}