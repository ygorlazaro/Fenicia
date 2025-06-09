using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface IUserRoleService
{
    Task<ServiceResponse<string[]>> GetRolesByUserAsync(Guid userId);
    Task<ServiceResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role);
}
