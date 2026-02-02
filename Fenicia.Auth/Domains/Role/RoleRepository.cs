using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

public class RoleRepository(AuthContext context, ILogger<RoleRepository> logger) : IRoleRepository
{
    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Attempting to retrieve Admin role");

            var adminRole = await context.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync(cancellationToken);

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
