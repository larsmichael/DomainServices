namespace DomainServices.Test
{
    using DomainServices.Authorization;
    using System.Collections.Generic;

    public class FakeEntitySecured : FakeEntity
    {
        public FakeEntitySecured(string id, string name, string group = null, IDictionary<string, object> metadata = null, IList<Permission> permissions = null)
            : base(id, name, group, metadata, permissions)
        {
            AddPermissions(new[] {"Administrators"}, new[] {"read", "update", "delete"});
            AddPermissions(new[] {"Editors"}, new[] {"read", "update"});
            AddPermissions(new[] {"Users"}, new[] {"read"});
        }
    }
}