using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.CheckCompanyExists;

public class CheckCompanyExistsHandler(AuthContext context)
{
    public async Task<bool> Handle(CheckUserExistsQuery checkUserExistsQuery, CancellationToken ct)
    {
        var query = context.Companies.Where(c => c.Cnpj == checkUserExistsQuery.Cnpj);

        if (checkUserExistsQuery.OnlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.AnyAsync();
    }
}