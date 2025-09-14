namespace Fenicia.Auth.Domains.Company;

using Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class CompanyRepository : ICompanyRepository
{
    private readonly AuthContext _authContext;
    private readonly ILogger<CompanyRepository> _logger;

    public CompanyRepository(AuthContext authContext, ILogger<CompanyRepository> logger)
    {
        _authContext = authContext;
        _logger = logger;
    }

    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            return await _authContext.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking company existence by ID {CompanyID}", companyId);
            throw;
        }
    }

    public async Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            return await _authContext.Companies.AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking company existence by CNPJ {Cnpj}", cnpj);
            throw;
        }
    }

    public CompanyModel Add(CompanyModel company)
    {
        _logger.LogInformation("Adding new company with CNPJ {Cnpj}", company.Cnpj);
        _authContext.Companies.Add(company);
        return company;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _authContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _authContext.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
            if (company == null)
            {
                _logger.LogInformation("Company not found for CNPJ {Cnpj}", cnpj);
            }

            return company;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company by CNPJ {Cnpj}", cnpj);
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
            _logger.LogError(ex, "Error retrieving companies for user {UserID}", userId);
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
            _logger.LogError(ex, "Error counting companies for user {UserID}", userId);
            throw;
        }
    }

    public CompanyModel PatchAsync(CompanyModel company)
    {
        _logger.LogInformation("Updating company with ID {CompanyID}", company.Id);
        _authContext.Entry(company).State = EntityState.Modified;
        return company;
    }

    private IQueryable<CompanyModel> QueryFromUserId(Guid userId)
    {
        var query = from company in _authContext.Companies join userRoles in _authContext.UserRoles on company.Id equals userRoles.CompanyId where userRoles.UserId == userId select company;
        return query;
    }

    public async Task<List<Guid>> GetCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = from company in _authContext.Companies
                    join userCompany in _authContext.UserRoles on company.Id equals userCompany.CompanyId
                    select userCompany.CompanyId;

        return await query.ToListAsync(cancellationToken: cancellationToken);
    }
}
