namespace Fenicia.Auth.Domains.Role.Logic;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class RoleRepository : IRoleRepository
{
    private readonly AuthContext _authContext;
    private readonly ILogger<RoleRepository> _logger;

    public RoleRepository(AuthContext authContext, ILogger<RoleRepository> logger)
    {
        _authContext = authContext;
        _logger = logger;
    }

    public async Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve Admin role");
            var adminRole = await _authContext.Roles.Where(role => role.Name == "Admin").FirstOrDefaultAsync(cancellationToken);

            if (adminRole == null)
            {
                _logger.LogWarning("Admin role not found in the database");
            }

            return adminRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving Admin role");
            throw;
        }
    }
}
