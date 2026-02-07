namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenService
{
    string GenerateRefreshToken(Guid userId);

    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken ct);

    Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken ct);
}