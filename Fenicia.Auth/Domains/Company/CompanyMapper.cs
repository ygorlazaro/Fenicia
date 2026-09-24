using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Company;
using CompanyResponseDTO = Fenicia.Common.DTOs.Auth.Company.CompanyResponse;
using UserRoleResponseDTO = Fenicia.Common.DTOs.Auth.UserRole.UserRoleResponse;

namespace Fenicia.Auth.Domains.Company;

/// <summary>
/// Mapper class for mapping between CompanyModel and CompanyResponse/CompanyRequest.
/// </summary>
public static class CompanyMapper
{
    /// <summary>
    /// Maps a UserRoleModel to a CompanyResponse, including the role name.
    /// </summary>
    /// <param name="userRole">The user role model.</param>
    /// <returns>The company response.</returns>
    public static CompanyResponseDTO MapToCompanyByUserResponse(UserRoleModel userRole)
    {
        return new CompanyResponseDTO(
            userRole.CompanyId,
            userRole.Company.Name,
            userRole.Company.Cnpj,
            userRole.Role.Name);
    }

    /// <summary>
    /// Maps a UserRoleResponse to a CompanyResponse, including the role name.
    /// </summary>
    /// <param name="userRole">The user role response.</param>
    /// <returns>The company response.</returns>
    public static CompanyResponseDTO MapToCompanyByUserResponse(UserRoleResponseDTO userRole)
    {
        return new CompanyResponseDTO(
            userRole.Company.Id,
            userRole.Company.Name,
            userRole.Company.Cnpj,
            userRole.Role);
    }

    /// <summary>
    /// Maps a CompanyModel to a CompanyResponse.
    /// </summary>
    /// <param name="company">The company model.</param>
    /// <returns>The company response.</returns>
    public static CompanyResponseDTO MapToCompanyResponse(CompanyModel company)
    {
        return new CompanyResponseDTO(company.Id, company.Name, company.Cnpj);
    }

    /// <summary>
    /// Maps a CompanyRequest to a CompanyModel.
    /// </summary>
    /// <param name="request">The company request.</param>
    /// <returns>The company model.</returns>
    public static CompanyModel MapToCompanyModel(CompanyRequest request)
    {
        return new CompanyModel
        {
            Id = request.Id,
            Name = request.Name,
            Cnpj = request.Cnpj,
            IsActive = true
        };
    }
}
