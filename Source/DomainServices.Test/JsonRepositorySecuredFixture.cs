namespace DomainServices.Test
{
    using System.Collections.Generic;
    using System.Security.Claims;

    public class JsonRepositorySecuredFixture
    {
        public JsonRepositorySecuredFixture()
        {
            var adminClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Admin"),
                new Claim(ClaimTypes.NameIdentifier, "admin"),
                new Claim(ClaimTypes.GroupSid, "Administrators"),
            };

            var adminIdentity = new ClaimsIdentity(adminClaims, "TestAuthType");
            Admin = new ClaimsPrincipal(adminIdentity);

            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "User"),
                new Claim(ClaimTypes.NameIdentifier, "user"),
                new Claim(ClaimTypes.GroupSid, "Users")
            };

            var userIdentity = new ClaimsIdentity(userClaims, "TestAuthType");
            User = new ClaimsPrincipal(userIdentity);

            var guestClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Guest"),
                new Claim(ClaimTypes.NameIdentifier, "guest"),
                new Claim(ClaimTypes.GroupSid, "Guests")
            };

            var guestIdentity = new ClaimsIdentity(guestClaims, "TestAuthType");
            Guest = new ClaimsPrincipal(guestIdentity);
        }

        public ClaimsPrincipal Admin { get; }

        public ClaimsPrincipal User { get; }

        public ClaimsPrincipal Guest { get; }
    }
}