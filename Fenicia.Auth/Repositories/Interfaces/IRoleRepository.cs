using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync();
}
