namespace DomainServices.Test.Authorization
{
    using System;
    using System.Collections.Generic;
    using DomainServices.Authorization;
    using Xunit;

    public class PermissionTest
    {
        [Fact]
        public void PrincipalsNullThrows()
        {
            Assert.Throws<ArgumentException>(() => new Permission(null, "read"));
        }

        [Fact]
        public void PrincipalsEmptyThrows()
        {
            Assert.Throws<ArgumentException>(() => new Permission(new HashSet<string>(), "read"));
        }

        [Fact]
        public void OperationNullThrows()
        {
            Assert.Throws<ArgumentNullException>(() => new Permission(new HashSet<string> { "Administrators", "Editors" }, null));
        }

        [Fact]
        public void CreateIsOk()
        {
            var permission = new Permission(new HashSet<string> {"Administrators", "Editors"}, "Read");
            Assert.Equal("read", permission.Operation);
            Assert.Equal("[Administrators, Editors] are allowed to read.", permission.ToString());
        }

        [Fact]
        public void CreateWithDuplicatesIsOk()
        {
            var permission = new Permission(new [] { "Administrators", "Administrators", "Editors" }, "Read");
            Assert.Equal(2, permission.Principals.Count);
            Assert.Equal("read", permission.Operation);
            Assert.Equal("[Administrators, Editors] are allowed to read.", permission.ToString());
        }
    }
}