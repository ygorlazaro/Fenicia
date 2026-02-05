using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Role;

public interface IRoleRepository : IBaseRepository<RoleModel>
{
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
