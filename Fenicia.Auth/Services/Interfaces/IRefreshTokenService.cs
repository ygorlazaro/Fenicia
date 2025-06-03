namespace Fenicia.Auth.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<string> GenerateRefreshTokenAsync(Guid userId);
    Task<bool> ValidateTokenAsync(Guid userId, string refreshToken);
    Task InvalidateRefreshTokenAsync(string refreshToken);
}