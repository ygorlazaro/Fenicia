using Fenicia.Auth.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole.Logic;

/// <summary>
/// Repository for managing user roles and their relationships
/// </summary>
public class UserRoleRepository(AuthContext context, ILogger<UserRoleRepository> logger) : IUserRoleRepository
{
    /// <summary>
    /// Retrieves all roles associated with a specific user
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Array of role names assigned to the user</returns>
    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving roles for user {UserId}", userId);
            return await context
                .UserRoles.Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.Name)
                .ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a user exists in a specific company
    /// </summary>
    /// <param name="userId">The unique identifier of the user</param>
    /// <param name="companyId">The unique identifier of the company</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>True if the user exists in the company, false otherwise</returns>
    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if user {UserId} exists in company {CompanyId}", userId, companyId);
            return await context.UserRoles.AnyAsync(ur =>
                ur.UserId == userId && ur.CompanyId == companyId,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if user {UserId} exists in company {CompanyId}", userId, companyId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a user has a specific role in a company
    /// </summary>
    /// <param name="guid">The unique identifier of the user</param>
    /// <param name="companyId">The unique identifier of the company</param>
    /// <param name="role">The role name to check</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>True if the user has the specified role in the company, false otherwise</returns>
    public async Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if user {UserId} has role {Role} in company {CompanyId}", guid, role, companyId);
            return await context.UserRoles.AnyAsync(ur =>
                ur.UserId == guid && ur.CompanyId == companyId && ur.Role.Name == role,
                cancellationToken
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if user {UserId} has role {Role} in company {CompanyId}", guid, role, companyId);
            throw;
        }
    }
}
