using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleRepository(AuthContext context, ILogger<UserRoleRepository> logger) : IUserRoleRepository
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Retrieving roles for user {UserID}", userId);

            return await context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving roles for user {UserID}", userId);

            throw;
        }
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if user {UserID} exists in company {CompanyID}", userId, companyId);

            return await context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if user {UserID} exists in company {CompanyID}", userId, companyId);

            throw;
        }
    }

    public async Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Checking if user {UserID} has role {Role} in company {CompanyID}", guid, role, companyId);

            return await context.UserRoles.AnyAsync(ur => ur.UserId == guid && ur.CompanyId == companyId && ur.Role.Name == role, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if user {UserID} has role {Role} in company {CompanyID}", guid, role, companyId);

            throw;
        }
    }

    public Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = from userRole in context.UserRoles
                    join company in context.Companies on userRole.CompanyId equals company.Id
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
