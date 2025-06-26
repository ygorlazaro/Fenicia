using Fenicia.Auth.Domains.Company.Data;

namespace Fenicia.Auth.Domains.Company.Logic;

/// <summary>
/// Repository interface for managing company data operations.
/// </summary>
public interface ICompanyRepository
{
    /// <summary>
    /// Checks if a company exists by its unique identifier.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>True if the company exists; otherwise, false.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken);
    /// <summary>
    /// Checks if a company exists by its CNPJ number.
    /// </summary>
    /// <param name="cnpj">The CNPJ number of the company.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>True if the company exists; otherwise, false.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken);
    /// <summary>
    /// Adds a new company to the repository.
    /// </summary>
    /// <param name="company">The company model to add.</param>
    /// <returns>The added company model.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    CompanyModel Add(CompanyModel company);
    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The number of state entries written to the database.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    Task<int> SaveAsync(CancellationToken cancellationToken);
    /// <summary>
    /// Retrieves a company by its CNPJ number.
    /// </summary>
    /// <param name="cnpj">The CNPJ number of the company to retrieve.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The company model if found; otherwise, null.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken);
    /// <summary>
    /// Retrieves a paginated list of companies associated with a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <param name="page">The page number (default: 1).</param>
    /// <param name="perPage">The number of items per page (default: 10).</param>
    /// <returns>A list of company models.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10);
    /// <summary>
    /// Updates an existing company in the repository.
    /// </summary>
    /// <param name="company">The company model with updated information.</param>
    /// <returns>The updated company model.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    CompanyModel PatchAsync(CompanyModel company);
    /// <summary>
    /// Counts the number of companies associated with a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The number of companies associated with the user.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("ReSharper", "UnusedMemberInSuper.Global")]
    Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
