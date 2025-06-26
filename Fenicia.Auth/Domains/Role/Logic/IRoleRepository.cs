using Fenicia.Auth.Domains.Role.Data;

namespace Fenicia.Auth.Domains.Role.Logic;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
