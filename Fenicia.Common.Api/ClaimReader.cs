using System.Security.Claims;

namespace Fenicia.Common.API;

public static class ClaimReader
{
    public static Guid UserId(ClaimsPrincipal user)
    {
        return GetGuidClaimValue(user, "userId");
    }

    public static void ValidateRole(ClaimsPrincipal user, string roleToSearch)
    {
        var access = user.Claims.Where(c => c.Type == "role").Any(x => x.Value == roleToSearch);

        if (!access) throw new UnauthorizedAccessException();
    }

    private static Guid GetGuidClaimValue(ClaimsPrincipal user, string claimType)
    {
        var claim = user.Claims.FirstOrDefault(c => string.Equals(c.Type, claimType, StringComparison.Ordinal));

        return claim == null ? throw new UnauthorizedAccessException() : Guid.Parse(claim.Value);
    }
}