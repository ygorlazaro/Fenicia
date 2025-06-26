using Fenicia.Common;

namespace Fenicia.Auth.Domains.UserRole.Logic;

/// <summary>
/// Service responsible for managing user role operations and interactions
/// </summary>
/// <remarks>
/// This service provides functionality for retrieving and validating user roles
/// </remarks>
public class UserRoleService(
    ILogger<UserRoleService> logger,
    IUserRoleRepository userRoleRepository
) : IUserRoleService
{
    /// <summary>
    /// Retrieves all roles associated with a specific user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>API response containing an array of role names</returns>
    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting to retrieve roles for user {UserId}", userId);

            var roles = await userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);

            logger.LogInformation("Successfully retrieved {RoleCount} roles for user {UserId}", roles.Length, userId);
            return new ApiResponse<string[]>(roles);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a user has a specific role within a company
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="companyId">The unique identifier of the company</param>
    /// <param name="role">The role name to check</param>
    /// <param name="cancellationToken">Token to cancel the operation if needed</param>
    /// <returns>API response indicating whether the user has the specified role</returns>
    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if user {UserId} has role {Role} in company {CompanyId}", userId, role, companyId);

            var response = await userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);

            logger.LogInformation("Role check completed for user {UserId}. Has role {Role}: {HasRole}", userId, role, response);
            return new ApiResponse<bool>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking role {Role} for user {UserId} in company {CompanyId}", role, userId, companyId);
            throw;
        }
    }
}
