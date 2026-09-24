using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Company;

namespace Fenicia.Auth.Domains.Company.Interfaces;

/// <summary>
/// Defines the contract for a service that manages company-related operations, including retrieval, updates, and insertion of company data.
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Retrieves a paginated list of companies associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to retrieve companies.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="perPage">The number of companies to retrieve per page.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The paginated list of companies.</returns>
    Task<Pagination<IEnumerable<CompanyResponse>>> GetCompaniesByUserAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the name of an existing company, ensuring that the user has administrative privileges for that company.
    /// </summary>
    /// <param name="companyId">The ID of the company to update.</param>
    /// <param name="userId">The ID of the user attempting the update.</param>
    /// <param name="name">The new name for the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(Guid companyId, Guid userId, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a company by its ID, returning a CompanyResponse if found, or null if not found.
    /// </summary>
    /// <param name="companyId">The ID of the company to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The CompanyResponse if found, or null if not found.</returns>
    Task<CompanyResponse?> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts a new company into the repository and returns a CompanyResponse representing the newly created company.
    /// </summary>
    /// <param name="request">The request containing the company details.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The CompanyResponse representing the newly created company.</returns>
    Task<CompanyResponse> InsertAsync(CompanyRequest request, CancellationToken cancellationToken = default);
}
