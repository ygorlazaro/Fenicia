using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Company;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Company;

/// <summary>
/// Service responsible for managing company-related operations, including retrieval, updates, and insertion of company data.
/// </summary>
/// <param name="repository">The repository for managing company data.</param>
/// <param name="userRoleService">The service for managing user role data.</param>
public sealed class CompanyService(
    ICompanyRepository repository,
    IUserRoleService userRoleService) : ICompanyService
{
    /// <summary>
    /// Retrieves a paginated list of companies associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to retrieve companies.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="perPage">The number of companies to retrieve per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A pagination object containing the list of companies and metadata.</returns>
    /// <exception cref="BadRequestException"></exception> <summary>
    /// </summary>
    public async Task<Pagination<IEnumerable<CompanyResponse>>> GetCompaniesByUserAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        if (perPage <= 0)
        {
            throw new BadRequestException(ExceptionMessages.UserNotAssociatedWithActiveCompanies);
        }

        var userRoles = await userRoleService.GetUserRolesAsync(userId, page, perPage, cancellationToken);
        var total = await userRoleService.CountUserRolesAsync(userId, cancellationToken);

        var activeUserRoles = userRoles.Where(ur => ur.Company.IsActive).ToList();
        var result = activeUserRoles.Select(CompanyMapper.MapToCompanyByUserResponse);

        return new Pagination<IEnumerable<CompanyResponse>>(result, total, page, perPage);
    }

    /// <summary>
    /// Updates the name of an existing company, ensuring that the user has administrative privileges for that company.
    /// </summary>
    /// <param name="companyId">The ID of the company to update.</param>
    /// <param name="userId">The ID of the user attempting the update.</param>
    /// <param name="name">The new name for the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="PermissionDeniedException"></exception>
    /// <exception cref="NotFoundException"></exception>
    public async Task UpdateAsync(
        Guid companyId,
        Guid userId,
        string name,
        CancellationToken cancellationToken = default)
    {
        var isAdmin = await userRoleService.IsAdminAsync(userId, companyId, cancellationToken);

        if (!isAdmin)
        {
            throw new PermissionDeniedException(ExceptionMessages.PermissionDeniedUpdateCompany);
        }

        var company = await repository.AnyActiveAsync(companyId, cancellationToken) ??
                      throw new NotFoundException(ExceptionMessages.CompanyNotFoundMessage);
        company.Name = name;
        await repository.UpdateAsync(company.Id, company, cancellationToken);
    }

    /// <summary>
    /// Retrieves a company by its ID, returning a CompanyResponse if found, or null if not found.
    /// </summary>
    /// <param name="companyId">The ID of the company to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The CompanyResponse if found, or null if not found.</returns>
    public async Task<CompanyResponse?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await repository.GetByIdAsync(companyId, cancellationToken);

        return company is null ? null : CompanyMapper.MapToCompanyResponse(company);
    }

    /// <summary>
    /// Inserts a new company into the repository and returns a CompanyResponse representing the newly created company.
    /// </summary>
    /// <param name="request">The request containing the company details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The CompanyResponse representing the newly created company.</returns>
    public async Task<CompanyResponse> InsertAsync(CompanyRequest request, CancellationToken cancellationToken = default)
    {
        var company = CompanyMapper.MapToCompanyModel(request);
        var result = await repository.InsertAsync(company, cancellationToken);

        return CompanyMapper.MapToCompanyResponse(result);
    }
}
