using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public static class CompanyExtensions
{

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

        public async Task<bool> AnyAsync(Guid companyId, CancellationToken ct)
        {
            var existing = await db.AnyAsync(c => c.Id == companyId, ct);

            return existing;
        }

        public async Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken ct)
        {
            var existing = await db.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, ct);

            return existing;
        }
    }
}