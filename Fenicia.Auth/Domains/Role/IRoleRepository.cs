using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.Role;

public interface IRoleRepository : IBaseRepository<RoleModel>
{
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
