using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(AuthContext context) : BaseRepository<CompanyModel>(context), ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, bool onlyActive, CancellationToken ct)
    {
        var query = context.Companies.Where(c => c.Id == companyId);

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, bool onlyActive, CancellationToken ct)
    {
        var query = context.Companies.Where(c => c.Cnpj == cnpj);

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.AnyAsync(ct);
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, bool onlyActive, CancellationToken ct)
    {
        var query = context.Companies.Where(c => c.Cnpj == cnpj);

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, bool onlyActive, CancellationToken ct, int page = 1, int perPage = 10)
    {
        var query = QueryFromUserId(userId, onlyActive);

        return await query.OrderBy(c => c.Name).Skip((page - 1) * perPage).Take(perPage).ToListAsync(ct);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, bool onlyActive, CancellationToken ct)
    {
        var query = QueryFromUserId(userId, onlyActive);

        return await query.CountAsync(ct);
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, bool onlyActive, CancellationToken ct)
    {
        var query = context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.CompanyId);

        if (onlyActive)
        {
            query = from companyId in query
                    join c in context.Companies on companyId equals c.Id
                    where c.IsActive
                    select companyId;
        }

        return await query.Distinct().ToListAsync(ct);
    }

    private IQueryable<CompanyModel> QueryFromUserId(Guid userId, bool onlyActive)
    {
        var query = from c in context.Companies
                    join ur in context.UserRoles on c.Id equals ur.CompanyId
                    where ur.UserId == userId
                    select c;

        if (onlyActive)
        {
            query = query.Where(c => c.IsActive);
        }

        return query;
    }
}