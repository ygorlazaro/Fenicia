using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;

namespace Fenicia.Auth.Services;

public class UserRoleService(ILogger<UserRoleService> logger, IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId)
    {
        logger.LogInformation("Getting roles by user id {userId}", userId);
        var roles = await userRoleRepository.GetRolesByUserAsync(userId);

        return roles;
    }

    public async Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role)
    {
        logger.LogInformation("Checking if user has role {role}", role);
        return await userRoleRepository.HasRoleAsync(userId, companyId, role);
    }
}