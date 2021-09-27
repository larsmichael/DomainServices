namespace DomainServices.Test
{
    public class FakeEntitySecured : FakeEntity
    {
        public FakeEntitySecured(string id, string name, string group = null) : base(id, name, group)
        {
            AddPermissions(new[] {"Administrators"}, new[] {"read", "update", "delete"});
            AddPermissions(new[] {"Editors"}, new[] {"read", "update"});
            AddPermissions(new[] {"Users"}, new[] {"read"});
        }
    }
}