namespace Fenicia.Auth.Domains.Role;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class RoleRepository : IRoleRepository
{
    private readonly AuthContext authContext;
    private readonly ILogger<RoleRepository> logger;

    public RoleRepository(AuthContext authContext, ILogger<RoleRepository> logger)
    {
        this.authContext = authContext;
        this.logger = logger;
    }

    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Attempting to retrieve Admin role");
            var adminRole = await this.authContext.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync(cancellationToken);

            if (adminRole == null)
            {
                this.logger.LogWarning("Admin role not found in the database");
            }

            return adminRole;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error occurred while retrieving Admin role");
            throw;
        }
    }
}
