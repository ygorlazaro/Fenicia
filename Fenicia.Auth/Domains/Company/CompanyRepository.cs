using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(DefaultContext context) : Repository<CompanyModel>(context)
{
    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj && c.Deleted == null, ct);
    }
}
