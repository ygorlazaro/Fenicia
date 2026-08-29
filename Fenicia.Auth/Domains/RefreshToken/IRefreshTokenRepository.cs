using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshTokenModel token, CancellationToken ct = default);

    Task<RefreshTokenModel?> GetAsync(string token, CancellationToken ct = default);

    Task UpdateAsync(RefreshTokenModel token, CancellationToken ct = default);
}
