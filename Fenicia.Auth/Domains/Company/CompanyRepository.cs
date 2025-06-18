using Fenicia.Auth.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(AuthContext authContext) : ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId)
    {
        return await authContext.Companies.AnyAsync(c => c.Id == companyId);
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj)
    {
        return await authContext.Companies.AnyAsync(c => c.Cnpj == cnpj);
    }

    public CompanyModel Add(CompanyModel company)
    {
        authContext.Companies.Add(company);

        return company;
    }

    public async Task<int> SaveAsync()
    {
        return await authContext.SaveChangesAsync();
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj)
    {
        return await authContext.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(
        Guid userId,
        int page = 1,
        int perPage = 10
    )
    {
        var query = QueryFromUserId(userId);

        return await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync();
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await QueryFromUserId(userId).CountAsync();
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
