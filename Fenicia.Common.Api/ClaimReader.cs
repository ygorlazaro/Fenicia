using System.Security.Claims;

namespace Fenicia.Common.Api;

public static class ClaimReader
{
    public static Guid UserId(ClaimsPrincipal user)
    {
        var claim = user.Claims.FirstOrDefault(claimToSearch => string.Equals(claimToSearch.Type, "userId", StringComparison.Ordinal));

        if (claim == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var value = Guid.Parse(claim.Value);

        return value;
    }

    public static Guid CompanyId(ClaimsPrincipal user)
    {
        var claim = user.Claims.FirstOrDefault(claimToSearch => string.Equals(claimToSearch.Type, "companyId", StringComparison.Ordinal));

        if (claim == null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var value = Guid.Parse(claim.Value);

        return value;
    }
}