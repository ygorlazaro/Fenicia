using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role;

public static class RoleMapper
{
    public static GetAdminRoleResponse MapToGetAdminRoleResponse(this RoleModel role)
    {
        return new GetAdminRoleResponse(role.Id, role.Name);
    }
}