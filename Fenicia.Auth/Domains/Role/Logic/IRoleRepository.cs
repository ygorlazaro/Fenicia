namespace Fenicia.Auth.Domains.Role.Logic;

using Common.Database.Models.Auth;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
