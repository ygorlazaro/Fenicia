using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Fenicia.Common.Enums.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(DefaultContext context) : Repository<CompanyModel>(context)
{
    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public async Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, cancellationToken);
    }

    public async Task<bool> AnyAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.Id == companyId, cancellationToken);
    }

    public async Task<bool> CheckExistsAsync(string cnpj, bool onlyActive, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(c => c.Cnpj == cnpj && (!onlyActive || c.IsActive), cancellationToken);
    }
}
