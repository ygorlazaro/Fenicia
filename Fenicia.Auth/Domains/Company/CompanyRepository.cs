namespace Fenicia.Auth.Domains.Company;

using Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class CompanyRepository : ICompanyRepository
{
    private readonly AuthContext authContext;
    private readonly ILogger<CompanyRepository> logger;

    public CompanyRepository(AuthContext authContext, ILogger<CompanyRepository> logger)
    {
        this.authContext = authContext;
        this.logger = logger;
    }

    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            return await this.authContext.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error checking company existence by ID {CompanyID}", companyId);
            throw;
        }
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            return await this.authContext.Companies.AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error checking company existence by CNPJ {Cnpj}", cnpj);
            throw;
        }
    }

    public CompanyModel Add(CompanyModel company)
    {
        this.logger.LogInformation("Adding new company with CNPJ {Cnpj}", company.Cnpj);
        this.authContext.Companies.Add(company);
        return company;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await this.authContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var company = await this.authContext.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
            if (company == null)
            {
                this.logger.LogInformation("Company not found for CNPJ {Cnpj}", cnpj);
            }

            return company;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving company by CNPJ {Cnpj}", cnpj);
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
            this.logger.LogError(ex, "Error retrieving companies for user {UserID}", userId);
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
            this.logger.LogError(ex, "Error counting companies for user {UserID}", userId);
            throw;
        }
    }

    public CompanyModel PatchAsync(CompanyModel company)
    {
        this.logger.LogInformation("Updating company with ID {CompanyID}", company.Id);
        this.authContext.Entry(company).State = EntityState.Modified;
        return company;
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = from company in this.authContext.Companies
                    join userCompany in this.authContext.UserRoles on company.Id equals userCompany.CompanyId
                    select userCompany.CompanyId;

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    private IQueryable<CompanyModel> QueryFromUserId(Guid userId)
    {
        var query = from company in this.authContext.Companies join userRoles in this.authContext.UserRoles on company.Id equals userRoles.CompanyId where userRoles.UserId == userId select company;
        return query;
    }
}
