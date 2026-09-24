using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Auth.Domains.Role;

/// <summary>
/// Provides mapping functionality for role entities and their corresponding response DTOs.
/// </summary>
public static class RoleMapper
{
    /// <summary>
    /// Maps a RoleModel entity to a RoleResponse DTO.
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public static RoleResponse MapToRoleResponse(RoleModel role)
    {
        return new RoleResponse(role.Id, role.Name);
    }
}
