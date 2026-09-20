using Fenicia.Auth.Domains.Company.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(DbContext context) : Repository<CompanyModel>(context), ICompanyRepository
{
    public Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public Task<CompanyModel?> AnyActiveAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Id == companyId && c.IsActive, cancellationToken);
    }
}
