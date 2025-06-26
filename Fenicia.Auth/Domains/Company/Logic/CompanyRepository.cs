using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Company.Data;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Company.Logic;

/// <summary>
/// Repository for managing company-related database operations
/// </summary>
public class CompanyRepository(AuthContext authContext, ILogger<CompanyRepository> logger) : ICompanyRepository
{
    /// <summary>
    /// Checks if a company exists by its ID
    /// </summary>
    /// <param name="companyId">The unique identifier of the company</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if company exists, false otherwise</returns>
    public async Task<bool> CheckCompanyExistsAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking company existence by ID {CompanyId}", companyId);
            throw;
        }
    }

    /// <summary>
    /// Checks if a company exists by its CNPJ
    /// </summary>
    /// <param name="cnpj">The CNPJ of the company</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if company exists, false otherwise</returns>
    public async Task<bool> CheckCompanyExistsAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.Companies.AnyAsync(c => c.Cnpj == cnpj, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking company existence by CNPJ {Cnpj}", cnpj);
            throw;
        }
    }

    /// <summary>
    /// Adds a new company to the context
    /// </summary>
    /// <param name="company">The company model to add</param>
    /// <returns>The added company model</returns>
    public CompanyModel Add(CompanyModel company)
    {
        logger.LogInformation("Adding new company with CNPJ {Cnpj}", company.Cnpj);
        authContext.Companies.Add(company);
        return company;
    }

    /// <summary>
    /// Saves all pending changes in the context
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The number of affected records</returns>
    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await authContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a company by its CNPJ
    /// </summary>
    /// <param name="cnpj">The CNPJ to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The company model if found, null otherwise</returns>
    public async Task<CompanyModel?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var company = await authContext.Companies.FirstOrDefaultAsync(c => c.Cnpj == cnpj, cancellationToken);
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

    /// <summary>
    /// Retrieves a paginated list of companies associated with a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <param name="page">The page number</param>
    /// <param name="perPage">The number of items per page</param>
    /// <returns>A list of company models</returns>
    public async Task<List<CompanyModel>> GetByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken,
        int page = 1,
        int perPage = 10
    )
    {
        try
        {
            var query = QueryFromUserId(userId);
            return await query.OrderBy(c => c.Name)
                            .Skip((page - 1) * perPage)
                            .Take(perPage)
                            .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving companies for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Counts the number of companies associated with a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of companies</returns>
    public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            return await QueryFromUserId(userId).CountAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error counting companies for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Updates an existing company in the context
    /// </summary>
    /// <param name="company">The company model to update</param>
    /// <returns>The updated company model</returns>
    public CompanyModel PatchAsync(CompanyModel company)
    {
        logger.LogInformation("Updating company with ID {CompanyId}", company.Id);
        authContext.Entry(company).State = EntityState.Modified;
        return company;
    }

    /// <summary>
    /// Creates a query to retrieve companies associated with a user
    /// </summary>
    /// <param name="userId">The user's ID</param>
    /// <returns>An IQueryable of company models</returns>
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
