using Fenicia.Auth.Repositories;

namespace Fenicia.Auth.Services;

public class UserUserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId)
    {
        var roles = await userRoleRepository.GetRolesByUserAsync(userId);

        return roles;
    }
}