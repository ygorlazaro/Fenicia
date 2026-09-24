using Fenicia.Common.DTOs.Auth.RefreshToken;

namespace Fenicia.Auth.Domains.RefreshToken;

/// <summary>
/// Provides mapping functionality for converting RefreshTokenModel instances to RefreshTokenResponse DTOs.
/// </summary>
public static class RefreshTokenMapper
{
    /// <summary>
    /// Maps a RefreshTokenModel instance to a RefreshTokenResponse DTO, extracting relevant properties such as the token string, expiration date, user ID, and active status.
    /// </summary>
    /// <param name="token">The RefreshTokenModel instance to map.</param>
    /// <returns>A RefreshTokenResponse DTO containing the mapped properties.</returns>
    public static RefreshTokenResponse MapToRefreshTokenResponse(RefreshTokenModel token)
    {
        return new RefreshTokenResponse(
            token.Token,
            token.ExpirationDate,
            token.UserId,
            token.IsActive);
    }
}
