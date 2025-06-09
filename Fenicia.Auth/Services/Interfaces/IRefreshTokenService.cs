using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<ServiceResponse<string>> GenerateRefreshTokenAsync(Guid userId);
    Task<ServiceResponse<bool>> ValidateTokenAsync(Guid userId, string refreshToken);
    Task<ServiceResponse<object>> InvalidateRefreshTokenAsync(string refreshToken);
}
