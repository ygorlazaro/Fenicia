using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync();
}