namespace Fenicia.Auth.Domains.RefreshToken.DTOs;

public record GenerateRefreshTokenResponse(string Token, DateTime ExpirationDate);
