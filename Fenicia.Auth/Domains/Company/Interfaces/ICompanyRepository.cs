using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Company.Interfaces;

/// <summary>
/// Defines the contract for a repository that manages company-related data operations, including retrieval and existence checks for active companies.
/// </summary>
public interface ICompanyRepository : IRepository<CompanyModel>
{
    /// <summary>
    /// Checks if there are any active companies associated with the specified company ID.
    /// </summary>
    /// <param name="companyId">The ID of the company to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The CompanyModel if found, or null if not found.</returns>
    Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken cancellationToken = default);
}
