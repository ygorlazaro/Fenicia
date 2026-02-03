using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(AuthContext context) : ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        return await context.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken)
    {
        return await context.Companies.AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public CompanyModel Add(CompanyModel company)
    {
        context.Companies.Add(company);

        return company;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        return await context.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
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
        context.Entry(company).State = EntityState.Modified;

        return company;
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.CompanyId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private IQueryable<CompanyModel> QueryFromUserId(Guid userId)
    {
        var query = from company in context.Companies join userRoles in context.UserRoles on company.Id equals userRoles.CompanyId where userRoles.UserId == userId select company;

        return query;
    }
}
