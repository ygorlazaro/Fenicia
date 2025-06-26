using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Role.Data;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role.Logic;

/// <summary>
/// Repository for managing role-related operations in the authentication system
/// </summary>
public class RoleRepository(AuthContext authContext, ILogger<RoleRepository> logger) : IRoleRepository
{
    /// <summary>
    /// Retrieves the Admin role from the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation if needed</param>
    /// <returns>The Admin role model if found, null otherwise</returns>
    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Attempting to retrieve Admin role");
            var adminRole = await authContext.Roles
                .Where(role => role.Name == "Admin")
                .FirstOrDefaultAsync(cancellationToken);

            if (adminRole == null)
            {
                logger.LogWarning("Admin role not found in the database");
            }

            return adminRole;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while retrieving Admin role");
            throw;
        }
    }
}
