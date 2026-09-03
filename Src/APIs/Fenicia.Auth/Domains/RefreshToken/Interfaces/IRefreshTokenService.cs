namespace Fenicia.Auth.Domains.RefreshToken.Interfaces;

public interface IRefreshTokenService
{
    Task<string> GenerateAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<RefreshTokenModel?> GetAsync(string token, CancellationToken cancellationToken = default);

    Task<RefreshTokenModel> UpdateAsync(string token, bool isActive, CancellationToken cancellationToken = default);

    Task InvalidateAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task<bool> ValidateAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
}