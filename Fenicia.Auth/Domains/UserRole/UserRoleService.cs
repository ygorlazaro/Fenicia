using Fenicia.Common;

using Fenicia.Common.Database.Models.Auth;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(ILogger<UserRoleService> logger, IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<ApiResponse<string[]>> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting to retrieve roles for user {UserID}", userId);

            var roles = await userRoleRepository.GetRolesByUserAsync(userId, cancellationToken);

            logger.LogInformation("Successfully retrieved {RoleCount} roles for user {UserID}", roles.Length, userId);

            return new ApiResponse<string[]>(roles);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving roles for user {UserID}", userId);

            throw;
        }
    }

    public async Task<ApiResponse<List<CompanyResponse>>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Starting to retrieve companies for user {UserID}", userId);

            var userRoles = await userRoleRepository.GetUserCompaniesAsync(userId, cancellationToken);

            logger.LogInformation("Successfully retrieved companies for user {UserID}", userId);

            return new ApiResponse<List<CompanyResponse>>(CompanyModel.Convert(userRoles));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving companies for user {UserID}", userId);

            throw;
        }
    }

    public async Task<ApiResponse<bool>> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if user {UserID} has role {Role} in company {CompanyID}", userId, role, companyId);

            var response = await userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);

            logger.LogInformation("Role check completed for user {UserID}. Has role {Role}: {HasRole}", userId, role, response);

            return new ApiResponse<bool>(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking role {Role} for user {UserID} in company {CompanyID}", role, userId, companyId);

            throw;
        }
    }
}
