using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories;

public class CompanyRepository(AuthContext authContext) : ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(string cnpj)
    {
        return await authContext.Companies.AnyAsync(c => c.CNPJ == cnpj);
    }

    public CompanyModel Add(CompanyModel company)
    {
        company.Created = DateTime.Now;
        authContext.Companies.Add(company);

        return company;
    }

    public async Task<int> SaveAsync()
    {
        return await authContext.SaveChangesAsync();
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj)
    {
        return await authContext.Companies.FirstOrDefaultAsync(c => c.CNPJ == cnpj);
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId)
    {
        var query = from company in authContext.Companies
            join userRoles in authContext.UserRoles on company.Id equals userRoles.CompanyId
            where userRoles.UserId == userId
            select company;

        return await query.ToListAsync();
    }
}