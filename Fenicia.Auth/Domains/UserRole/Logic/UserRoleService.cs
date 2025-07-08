namespace Fenicia.Auth.Domains.UserRole.Logic;

using Common;

public class UserRoleService(ILogger<UserRoleService> logger, IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Starting to retrieve roles for user {UserId}", userId);

            var roles = await userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);

            logger.LogInformation(message: "Successfully retrieved {RoleCount} roles for user {UserId}", roles.Length, userId);
            return new ApiResponse<string[]>(roles);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving roles for user {UserId}", userId);
            throw;
        }
    }

    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Checking if user {UserId} has role {Role} in company {CompanyId}", userId, role, companyId);

            var response = await userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);

            logger.LogInformation(message: "Role check completed for user {UserId}. Has role {Role}: {HasRole}", userId, role, response);
            return new ApiResponse<bool>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error checking role {Role} for user {UserId} in company {CompanyId}", role, userId, companyId);
            throw;
        }
    }
}
