using Fenicia.Auth.Repositories.Interfaces;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class UserRoleService(
    ILogger<UserRoleService> logger,
    IUserRoleRepository userRoleRepository
) : IUserRoleService
{
    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId)
    {
        logger.LogInformation("Getting roles by user id {userId}", userId);
        var roles = await userRoleRepository.GetRolesByUserAsync(userId);

        return new ApiResponse<string[]>(roles);
    }

    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role)
    {
        logger.LogInformation("Checking if user has role {role}", role);
        var response = await userRoleRepository.HasRoleAsync(userId, companyId, role);

        return new ApiResponse<bool>(response);
    }
}
