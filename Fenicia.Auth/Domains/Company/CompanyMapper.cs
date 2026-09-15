using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Company;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Company;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CompanyMapper
{
    [MapProperty(nameof(UserRoleModel.CompanyId),  nameof(GetCompaniesByUserResponse.Id))]
    [MapProperty([nameof(UserRoleModel.Company), nameof(UserRoleModel.Company.Name)],  nameof(GetCompaniesByUserResponse.Name))]
    [MapProperty([nameof(UserRoleModel.Company), nameof(UserRoleModel.Company.Cnpj)],  nameof(GetCompaniesByUserResponse
        .Cnpj))]
    [MapProperty([nameof(UserRoleModel.Company), nameof(UserRoleModel.Role.Name)],  nameof(GetCompaniesByUserResponse
        .Role))]
    public partial GetCompaniesByUserResponse MapToGetCompaniesByUserResponse(UserRoleModel userRole);
}