namespace Fenicia.Auth.Domains.RefreshToken;

using Common.Database.Models.Auth;

public interface IRefreshTokenRepository
{
    void Add(RefreshTokenModel refreshToken);

    Task SaveChangesAsync(CancellationToken cancellationToken);

    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken);

    Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
