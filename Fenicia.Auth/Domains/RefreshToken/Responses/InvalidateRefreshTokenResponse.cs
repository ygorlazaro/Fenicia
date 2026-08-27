namespace Fenicia.Auth.Domains.RefreshToken.Responses;

public record InvalidateRefreshTokenResponse(string Token, DateTime ExpirationDate, Guid UserId)
{

    public bool IsActive { get; set; }
}