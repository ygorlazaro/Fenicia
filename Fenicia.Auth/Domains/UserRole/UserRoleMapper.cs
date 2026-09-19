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

    [MapProperty("Company.Id", nameof(UserCompanyResponse.Id))]
    [MapProperty("Role.Name", nameof(UserCompanyResponse.Role))]
    [MapProperty(nameof(UserRoleModel.CompanyId), nameof(UserCompanyResponse.CompanyId))]
    [MapProperty("Company.Name", nameof(UserCompanyResponse.CompanyName))]
    [MapProperty("Company.Cnpj", nameof(UserCompanyResponse.Cnpj))]
    public partial UserCompanyResponse MapToGetUserCompaniesResponse(UserRoleModel userRole);
}
