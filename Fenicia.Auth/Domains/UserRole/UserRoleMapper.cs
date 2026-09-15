using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.UserRole;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.UserRole;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class UserRoleMapper
{
    [MapProperty("Role.Name", nameof(UserRoleResponse.Role))]
    [MapProperty("Company.Id", nameof(UserRoleResponse.Company) + "." + nameof(CompanyResponse.Id))]
    [MapProperty("Company.Name", nameof(UserRoleResponse.Company) + "." + nameof(CompanyResponse.Name))]
    [MapProperty("Company.Cnpj", nameof(UserRoleResponse.Company) + "." + nameof(CompanyResponse.Cnpj))]
    public partial UserRoleResponse MapToUserRoleResponse(UserRoleModel userRole);

    [MapProperty("Company.Id", nameof(GetUserCompaniesResponse.Id))]
    [MapProperty("Role.Name", nameof(GetUserCompaniesResponse.Role))]
    [MapProperty(nameof(UserRoleModel.CompanyId), nameof(GetUserCompaniesResponse.CompanyId))]
    [MapProperty("Company.Name", nameof(GetUserCompaniesResponse.CompanyName))]
    [MapProperty("Company.Cnpj", nameof(GetUserCompaniesResponse.Cnpj))]
    public partial GetUserCompaniesResponse MapToGetUserCompaniesResponse(UserRoleModel userRole);
}
