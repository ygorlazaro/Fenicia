using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Company;

namespace Fenicia.Auth.Domains.Company;

public static class CompanyMapper
{
    public static GetCompaniesByUserResponse MapToGetCompaniesByUserResponse(this UserRoleModel userRole)
    {
        return new GetCompaniesByUserResponse(
            userRole.Company.Id,
            userRole.Company.Name,
            userRole.Company.Cnpj,
            userRole.Role.Name);
    }
}