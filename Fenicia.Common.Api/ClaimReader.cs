using System.Security.Claims;

namespace Fenicia.Common.Api;

public static class ClaimReader
{
    public static Guid UserId(ClaimsPrincipal user) => GetGuidClaimValue(user, "userId");

    public static Guid CompanyId(ClaimsPrincipal user) => GetGuidClaimValue(user, "companyId");

    private static Guid GetGuidClaimValue(ClaimsPrincipal user, string claimType)
    {
        var claim = user.Claims.FirstOrDefault(claimToSearch => 
            string.Equals(claimToSearch.Type, claimType, StringComparison.Ordinal));

        return claim == null ? throw new UnauthorizedAccessException() : Guid.Parse(claim.Value);
    }

    public static string[] Modules(ClaimsPrincipal user)
    {
        return user.Claims.Where(x => x.Type == "module").Select(x => x.Value).ToArray();
    }

    public static void ValidateRole(ClaimsPrincipal user, string 
        roleToSearch)
    {
        var access = user.Claims.Where(x => x.Type == "role").Any(x => x.Value == roleToSearch);

        if (!access)
        {
            throw new UnauthorizedAccessException();
        }
    }
}