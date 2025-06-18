namespace Fenicia.Auth.Domains.Role;

public interface IRoleRepository
{
    Task<RoleModel?> GetAdminRoleAsync();
}
