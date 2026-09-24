using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.UserRole;

/// <summary>
/// Mapper class for mapping between UserRoleModel and UserRoleResponse/UserCompanyResponse.
/// </summary>
public static class UserRoleMapper
{
    /// <summary>
    /// Maps a UserRoleModel to a UserRoleResponse, including the company details.
    /// </summary>
    /// <param name="userRole">The user role model.</param>
    /// <returns>The user role response.</returns>
    public static UserRoleResponse MapToUserRoleResponse(UserRoleModel userRole)
    {
        return new UserRoleResponse(
            userRole.Id,
            userRole.Role.Name,
            new CompanyResponse(
                userRole.Company.Id,
                userRole.Company.Name,
                userRole.Company.Cnpj,
                userRole.Company.IsActive));
    }

    /// <summary>
    /// Maps a UserRoleModel to a UserCompanyResponse.
    /// </summary>
    /// <param name="userRole">The user role model.</param>
    /// <returns>The user company response.</returns>
    public static UserCompanyResponse MapToUserCompanyResponse(UserRoleModel userRole)
    {
        return new UserCompanyResponse(
            userRole.Company.Id,
            userRole.Role.Name,
            userRole.CompanyId,
            userRole.Company.Name,
            userRole.Company.Cnpj);
    }
}