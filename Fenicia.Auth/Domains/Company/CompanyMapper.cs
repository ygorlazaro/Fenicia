using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Company;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Auth.Domains.Company;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CompanyMapper
{
    [MapProperty(nameof(UserRoleModel.CompanyId),  nameof(CompanyByUserResponse.Id))]
    [MapProperty([nameof(UserRoleModel.Company), nameof(UserRoleModel.Company.Name)],  nameof(CompanyByUserResponse.Name))]
    [MapProperty([nameof(UserRoleModel.Company), nameof(UserRoleModel.Company.Cnpj)],  nameof(CompanyByUserResponse
        .Cnpj))]
    [MapProperty([nameof(UserRoleModel.Company), nameof(UserRoleModel.Role.Name)],  nameof(CompanyByUserResponse
        .Role))]
    public partial CompanyByUserResponse MapToCompaniesByUserResponse(UserRoleModel userRole);

    public partial CompanyResponse MapToCompanyResponse(CompanyModel company);
}