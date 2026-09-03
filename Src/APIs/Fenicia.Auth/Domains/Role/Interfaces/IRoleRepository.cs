using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Role.Interfaces;

public interface IRoleRepository : IRepository<RoleModel>
{
    Task<RoleModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}