using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public static class CompanyExtensions
{
    extension(DbSet<CompanyModel> db)
    {
        public async Task<bool> AnyCnpjAsync(string cnpj, CancellationToken ct, bool onlyActive = true)
        {
            var companies = db.Where(c => c.Cnpj == cnpj);

            if (onlyActive)
            {
                companies = companies.Where(c => c.IsActive);
            }

            return await companies.AnyAsync(ct);
        }

        public async Task ValidateExistingAsync(Guid companyId, CancellationToken ct)
        {
            var existing = await db.AnyAsync(c => c.Id == companyId, ct);

            if (!existing)
            {
                throw new InvalidRequestException(ExceptionMessages.CompanyNotFoundMessage);
            }
        }
    }
}