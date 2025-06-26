using Fenicia.Common;

namespace Fenicia.Auth.Domains.UserRole.Logic;

public class UserRoleService(
    ILogger<UserRoleService> logger,
    IUserRoleRepository userRoleRepository
) : IUserRoleService
{
    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting roles by user id {userId}", userId);
        var roles = await userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);

        return new ApiResponse<string[]>(roles);
    }

    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking if user has role {role}", role);
        var response = await userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);

        return new ApiResponse<bool>(response);
    }
}
