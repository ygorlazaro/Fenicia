using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(DefaultContext context) : Repository<CompanyModel>(context)
{
    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj && c.Deleted == null, ct);
    }

    public async Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken ct = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, ct);
    }

    public async Task<bool> AnyAsync(Guid companyId, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(c => c.Id == companyId, ct);
    }

    public async Task<bool> CheckExistsAsync(string cnpj, bool onlyActive, CancellationToken ct = default)
    {
        return await DbSet.AnyAsync(c => c.Cnpj == cnpj && (!onlyActive || c.IsActive), ct);
    }
}
