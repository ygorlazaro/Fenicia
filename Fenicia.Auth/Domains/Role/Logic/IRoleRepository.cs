namespace Fenicia.Auth.Domains.Role.Logic;

using Data;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
