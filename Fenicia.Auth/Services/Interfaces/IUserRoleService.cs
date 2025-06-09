using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface IUserRoleService
{
    Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId);
    Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role);
}
