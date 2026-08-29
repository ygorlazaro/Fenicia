using Fenicia.Auth.Domains.Company.DTOs;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Company;

public static partial class CompanyMapper
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
