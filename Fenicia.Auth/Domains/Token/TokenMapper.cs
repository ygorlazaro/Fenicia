using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Token;

namespace Fenicia.Auth.Domains.Token;

/// <summary>
/// Mapper class for mapping between TokenRequest and TokenResponse.
/// </summary>
public static class TokenMapper
{
    /// <summary>
    /// Maps a TokenRequest to a TokenResponse.
    /// </summary>
    /// <param name="request">The token request.</param>
    /// <param name="userId">The user ID.</param>
    /// <param name="name">The user name.</param>
    /// <param name="companyId">The user's company ID.</param>
    /// <param name="roles">The user's roles.</param>
    /// <returns>The token response.</returns>
    public static TokenResponse MapToTokenResponse(TokenRequest request, Guid userId, string name, Guid companyId, List<string> roles)
    {
        return new TokenResponse(userId, name, request.Email, companyId, roles);
    }
}
