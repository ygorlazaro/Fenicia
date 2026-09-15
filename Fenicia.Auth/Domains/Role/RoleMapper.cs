using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Role;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Role;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class RoleMapper
{
    internal partial GetAdminRoleResponse MapToGetAdminRoleResponse(RoleModel role);
}
