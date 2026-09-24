using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

/// <summary>
/// Initializes a new instance of the <see cref="CompanyRepository"/> class.
/// </summary>
/// <param name="context">The database context.</param>
public class CompanyRepository(DbContext context) : Repository<CompanyModel>(context), ICompanyRepository
{
    /// <summary>
    /// Checks if there are any active companies associated with the specified company ID.
    /// </summary>
    /// <param name="companyId">The ID of the company to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The CompanyModel if found, or null if not found.</returns>
    public Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, cancellationToken);
    }
}
