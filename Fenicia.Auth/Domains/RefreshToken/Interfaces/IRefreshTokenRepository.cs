namespace Fenicia.Auth.Domains.RefreshToken.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshTokenModel token);

    Task<RefreshTokenModel?> GetAsync(string token);

    Task UpdateAsync(RefreshTokenModel token);
}
