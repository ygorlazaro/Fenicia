using Fenicia.Auth.Domains.Company.Data;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Company.Logic;

/// <summary>
/// Provides services for managing company-related operations.
/// </summary>
public interface ICompanyService
{
    /// <summary>
    /// Retrieves a company by its CNPJ (Brazilian National Registry of Legal Entities).
    /// </summary>
    /// <param name="cnpj">The CNPJ number of the company to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>An API response containing the company information if found.</returns>
    Task<ApiResponse<CompanyResponse>> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a paginated list of companies associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <param name="page">The page number for pagination (default is 1).</param>
    /// <param name="perPage">The number of items per page (default is 10).</param>
    /// <returns>An API response containing a list of companies.</returns>
    Task<ApiResponse<List<CompanyResponse>>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken,
        int page = 1,
        int perPage = 10
    );

    /// <summary>
    /// Updates specific properties of a company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company to update.</param>
    /// <param name="userId">The unique identifier of the user performing the update.</param>
    /// <param name="company">The company update request containing the modified properties.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>An API response containing the updated company information.</returns>
    Task<ApiResponse<CompanyResponse>> PatchAsync(
        Guid companyId,
        Guid userId,
        CompanyUpdateRequest company,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Counts the number of companies associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>An API response containing the count of companies.</returns>
    Task<ApiResponse<int>> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
