using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    void Add(RefreshTokenModel refreshToken);
    Task SaveChangesAsync();
    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken);
    Task InvalidateRefreshTokenAsync(string refreshToken);
}
