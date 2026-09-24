using Fenicia.Auth.Domains.Role.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Role;

/// <summary>
/// Represents a repository for managing role entities in the authentication domain.
/// </summary>
/// <param name="context"></param>
public class RoleRepository(DbContext context) : Repository<RoleModel>(context), IRoleRepository
{
    /// <summary>
    /// Retrieves a role entity by its name.
    /// </summary>
    /// <param name="name">The name of the role to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The role entity if found; otherwise, null.</returns>
    public Task<RoleModel?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }

    /// <summary>
    /// Retrieves a list of roles by their IDs.
    /// </summary>
    /// <param name="roleIds">The IDs of the roles to retrieve.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of role entities.</returns>
    public Task<List<RoleModel>> GetRolesByIdAsync(List<Guid> roleIds, CancellationToken cancellationToken)
    {
        var query = from r in DbSet
                    where roleIds.Contains(r.Id)
                    select r;

        return query.ToListAsync(cancellationToken);
    }
}
