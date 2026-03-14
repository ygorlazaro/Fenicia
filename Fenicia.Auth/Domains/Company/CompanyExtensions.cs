using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

/// <summary>
///     Extension methods for Company-related database operations.
///     Provides convenient query methods for checking company existence by CNPJ or ID.
/// </summary>
public static class CompanyExtensions
{
    /// <summary>
    ///     Checks if any company exists with the given CNPJ.
    /// </summary>
    /// <param name="db">The DbSet of CompanyModel.</param>
    /// <param name="cnpj">The CNPJ to search for.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="onlyActive">Whether to consider only active companies (default: true).</param>
    /// <returns>True if a company with the given CNPJ exists, otherwise false.</returns>
    extension(DbSet<CompanyModel> db)
    {
        public async Task<bool> AnyCnpjAsync(string cnpj, CancellationToken ct, bool onlyActive = true)
        {
            var companies = db.Where(c => c.Cnpj == cnpj);

            companies = onlyActive switch
            {
                true => companies.Where(c => c.IsActive),
                _ => companies
            };

            return await companies.AnyAsync(ct);
        }

        /// <summary>
        ///     Checks if any company exists with the given company ID.
        /// </summary>
        /// <param name="db">The DbSet of CompanyModel.</param>
        /// <param name="companyId">The company ID to search for.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>True if a company with the given ID exists, otherwise false.</returns>
        public async Task<bool> AnyAsync(Guid companyId, CancellationToken ct)
        {
            var existing = await db.AnyAsync(c => c.Id == companyId, ct);

            return existing;
        }

        /// <summary>
        ///     Checks if any active company exists with the given company ID.
        /// </summary>
        /// <param name="db">The DbSet of CompanyModel.</param>
        /// <param name="companyId">The company ID to search for.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The CompanyModel if found and active, otherwise null.</returns>
        public async Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken ct)
        {
            var existing = await db.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, ct);

            return existing;
        }
    }
}