namespace Fenicia.Auth.Domains.Company.Logic;

using Contexts;

using Data;

using Microsoft.EntityFrameworkCore;

public class CompanyRepository(AuthContext authContext, ILogger<CompanyRepository> logger) : ICompanyRepository
{
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error checking company existence by ID {CompanyId}", companyId);
            throw;
        }
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.Companies.AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error checking company existence by CNPJ {Cnpj}", cnpj);
            throw;
        }
    }

    public CompanyModel Add(CompanyModel company)
    {
        logger.LogInformation(message: "Adding new company with CNPJ {Cnpj}", company.Cnpj);
        authContext.Companies.Add(company);
        return company;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error saving changes to database");
            throw;
        }
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var company = await authContext.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
            if (company == null)
            {
                logger.LogInformation(message: "Company not found for CNPJ {Cnpj}", cnpj);
            }

            return company;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving company by CNPJ {Cnpj}", cnpj);
            throw;
        }
    }

    public async Task<List<CompanyModel>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        try
        {
            var query = QueryFromUserId(userId);
            return await query.OrderBy(c => c.Name).Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving companies for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return await QueryFromUserId(userId).CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error counting companies for user {UserId}", userId);
            throw;
        }
    }

    public CompanyModel PatchAsync(CompanyModel company)
    {
        logger.LogInformation(message: "Updating company with ID {CompanyId}", company.Id);
        authContext.Entry(company).State = EntityState.Modified;
        return company;
    }

    private IQueryable<CompanyModel> QueryFromUserId(Guid userId)
    {
        var query = from company in authContext.Companies join userRoles in authContext.UserRoles on company.Id equals userRoles.CompanyId where userRoles.UserId == userId select company;
        return query;
    }
}
