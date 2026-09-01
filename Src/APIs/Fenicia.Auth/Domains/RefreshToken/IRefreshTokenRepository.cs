namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshTokenModel token, CancellationToken cancellationToken = default);

    Task<RefreshTokenModel?> GetAsync(string token, CancellationToken cancellationToken = default);

    Task UpdateAsync(RefreshTokenModel token, CancellationToken cancellationToken = default);
}
