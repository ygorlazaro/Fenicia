namespace Fenicia.Auth.Domains.Role.Logic;

using Contexts;

using Data;

using Microsoft.EntityFrameworkCore;

public class RoleRepository(AuthContext authContext, ILogger<RoleRepository> logger) : IRoleRepository
{
    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Attempting to retrieve Admin role");
            var adminRole = await authContext.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync(cancellationToken);

            if (adminRole == null)
            {
                logger.LogWarning(message: "Admin role not found in the database");
            }

            return adminRole;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error occurred while retrieving Admin role");
            throw;
        }
    }
}
