using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(AuthContext context) : BaseRepository<CompanyModel>(context), ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, bool onlyActive, CancellationToken cancellationToken)
    {
        var query = context.Companies.Where(c => c.Id == companyId);

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, bool onlyActive, CancellationToken cancellationToken)
    {
        var query = context.Companies.Where(c => c.Cnpj == cnpj);

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, bool onlyActive, CancellationToken cancellationToken)
    {
        var query = context.Companies.Where(c => c.Cnpj == cnpj);

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, bool onlyActive, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var query = QueryFromUserId(userId, onlyActive);

        return await query.OrderBy(c => c.Name).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, bool onlyActive, CancellationToken cancellationToken)
    {
        var query = QueryFromUserId(userId, onlyActive);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, bool onlyActive, CancellationToken cancellationToken)
    {
        var query = context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.CompanyId);

        if (onlyActive)
        {
            query = from companyId in query
                    join company in context.Companies on companyId equals company.Id
                    where company.IsActive
                    select companyId;
        }

        return await query.Distinct().ToListAsync(cancellationToken);
    }

    private IQueryable<CompanyModel> QueryFromUserId(Guid userId, bool onlyActive)
    {
        var query = from company in context.Companies
                    join userRoles in context.UserRoles on company.Id equals userRoles.CompanyId
                    where userRoles.UserId == userId
                    select company;

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return query;
    }
}
