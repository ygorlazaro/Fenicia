using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class UserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId)
    {
        var roles = await userRoleRepository.GetRolesByUserAsync(userId);

        return roles;
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role)
    {
        return await userRoleRepository.HasRoleAsync(userId, companyId, role);
    }
}