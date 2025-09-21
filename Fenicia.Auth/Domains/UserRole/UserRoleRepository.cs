namespace Fenicia.Auth.Domains.UserRole;

using System.Collections.Generic;

using Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AuthContext authContext;
    private readonly ILogger<UserRoleRepository> logger;

    public UserRoleRepository(AuthContext context, ILogger<UserRoleRepository> logger)
    {
        this.authContext = context;
        this.logger = logger;
    }

    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Retrieving roles for user {UserID}", userId);
            return await this.authContext.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving roles for user {UserID}", userId);
            throw;
        }
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Checking if user {UserID} exists in company {CompanyID}", userId, companyId);
            return await this.authContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error checking if user {UserID} exists in company {CompanyID}", userId, companyId);
            throw;
        }
    }

    public async Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            this.logger.LogInformation("Checking if user {UserID} has role {Role} in company {CompanyID}", guid, role, companyId);
            return await this.authContext.UserRoles.AnyAsync(ur => ur.UserId == guid && ur.CompanyId == companyId && ur.Role.Name == role, cancellationToken);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error checking if user {UserID} has role {Role} in company {CompanyID}", guid, role, companyId);
            throw;
        }
    }

    public Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = from userRole in authContext.UserRoles
                    join company in authContext.Companies on userRole.CompanyId equals company.Id
                    where userRole.UserId == userId
                    select new UserRoleModel
                    {
                        Id = company.Id,
                        Role = userRole.Role,
                        Company = new CompanyModel
                        {
                            Id = company.Id,
                            Name = company.Name,
                            Cnpj = company.Cnpj
                        }
                    };

        return query.ToListAsync(cancellationToken);
    }
}
