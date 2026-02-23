using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.CheckCompanyExists;

public class CheckCompanyExistsHandler(AuthContext context)
{
    public virtual async Task<bool> Handle(CheckCompanyExistsQuery query, CancellationToken ct)
    {
        var companies = context.Companies.Where(c => c.Cnpj == query.Cnpj);

        if (query.OnlyActive) companies = companies.Where(c => c.IsActive);

        return await companies.AnyAsync(ct);
    }
}