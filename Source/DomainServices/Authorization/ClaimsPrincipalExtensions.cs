namespace DomainServices.Authorization;

using System.Collections.Generic;
using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
    }

    public static HashSet<string> GetPrincipals(this ClaimsPrincipal user)
    {
        var principals = new HashSet<string> { user.GetUserId() };
        foreach (var claim in user.Claims)
        {
            if (claim.Type == ClaimTypes.GroupSid)
            {
                principals.Add(claim.Value);
            }
        }
        return principals;
    }
}