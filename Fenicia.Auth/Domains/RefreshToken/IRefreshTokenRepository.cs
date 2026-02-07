namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);

    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken ct);

    Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken ct);
}