using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(DefaultContext context) : Repository<CompanyModel>(context), ICompanyRepository
{
    public Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, cancellationToken);
    }

    public Task<bool> AnyAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(c => c.Id == companyId, cancellationToken);
    }

    public Task<bool> CheckExistsAsync(
        string cnpj,
        bool onlyActive,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(c => c.Cnpj == cnpj && (!onlyActive || c.IsActive), cancellationToken);
    }
}