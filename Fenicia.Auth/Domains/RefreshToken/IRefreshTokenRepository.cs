namespace Fenicia.Auth.Domains.RefreshToken;

public interface IRefreshTokenRepository
{
    void Add(RefreshTokenModel refreshToken);
    Task SaveChangesAsync();
    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken);
    Task InvalidateRefreshTokenAsync(string refreshToken);
}
