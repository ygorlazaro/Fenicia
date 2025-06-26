using Fenicia.Auth.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.Logic;

public class CompanyRepository(AuthContext authContext) : ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await authContext.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken)
    {
        return await authContext.Companies.AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public CompanyModel Add(CompanyModel company)
    {
        authContext.Companies.Add(company);

        return company;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await authContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        return await authContext.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken,
        int page = 1,
        int perPage = 10
    )
    {
        var query = QueryFromUserId(userId);

        return await query.OrderBy(c => c.Name).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await QueryFromUserId(userId).CountAsync(cancellationToken);
    }

    public CompanyModel PatchAsync(CompanyModel company)
    {
        authContext.Entry(company).State = EntityState.Modified;

        return company;
    }

    private IQueryable<CompanyModel> QueryFromUserId(Guid userId)
    {
        var query =
            from company in authContext.Companies
            join userRoles in authContext.UserRoles on company.Id equals userRoles.CompanyId
            where userRoles.UserId == userId
            select company;
        return query;
    }
}
