using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Role;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
