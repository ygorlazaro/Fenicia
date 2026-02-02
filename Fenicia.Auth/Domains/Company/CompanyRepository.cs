using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company;

public class CompanyRepository(AuthContext context, ILogger<CompanyRepository> logger) : ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking company existence by ID {CompanyID}", companyId);

            throw;
        }
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            return await context.Companies.AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking company existence by CNPJ {Cnpj}", cnpj);

            throw;
        }
    }

    public CompanyModel Add(CompanyModel company)
    {
        logger.LogInformation("Adding new company with CNPJ {Cnpj}", company.Cnpj);

        context.Companies.Add(company);

        return company;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving changes to database");

            throw;
        }
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var company = await context.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);

            if (company == null)
            {
                logger.LogInformation("Company not found for CNPJ {Cnpj}", cnpj);
            }

            return company;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving company by CNPJ {Cnpj}", cnpj);

            throw;
        }
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        try
        {
            var query = this.QueryFromUserId(userId);

            return await query.OrderBy(c => c.Name).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving companies for user {UserID}", userId);

            throw;
        }
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return await this.QueryFromUserId(userId).CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting companies for user {UserID}", userId);

            throw;
        }
    }

    public CompanyModel PatchAsync(CompanyModel company)
    {
        logger.LogInformation("Updating company with ID {CompanyID}", company.Id);

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
