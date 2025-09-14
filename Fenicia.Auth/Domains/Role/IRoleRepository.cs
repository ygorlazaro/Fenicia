namespace Fenicia.Auth.Domains.Role;

using Common.Database.Models.Auth;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
