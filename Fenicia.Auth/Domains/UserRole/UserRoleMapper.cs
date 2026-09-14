using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.UserRole;

public static class UserRoleMapper
{
    public static UserRoleResponse MapToUserRoleResponse(this UserRoleModel userRole)
    {
        return new UserRoleResponse(
            userRole.CompanyId,
            userRole.Role.Name,
            new CompanyResponse(userRole.CompanyId, userRole.Company.Name, userRole.Company.Cnpj));
    }

    public static GetUserCompaniesResponse MapToGetUserCompaniesResponse(this UserRoleModel userRole)
    {
        return new GetUserCompaniesResponse(
            userRole.CompanyId,
            userRole.Role.Name,
            userRole.CompanyId,
            userRole.Company.Name,
            userRole.Company.Cnpj);
    }
}