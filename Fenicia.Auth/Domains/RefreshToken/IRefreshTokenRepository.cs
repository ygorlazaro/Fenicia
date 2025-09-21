namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenRepository
{
    void Add(RefreshToken refreshToken);

    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);

    Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
