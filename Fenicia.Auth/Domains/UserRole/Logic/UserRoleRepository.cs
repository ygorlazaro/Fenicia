namespace Fenicia.Auth.Domains.UserRole.Logic;

using Contexts;

using Microsoft.EntityFrameworkCore;

public class UserRoleRepository(AuthContext context, ILogger<UserRoleRepository> logger) : IUserRoleRepository
{
    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Retrieving roles for user {UserId}", userId);
            return await context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving roles for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Checking if user {UserId} exists in company {CompanyId}", userId, companyId);
            return await context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error checking if user {UserId} exists in company {CompanyId}", userId, companyId);
            throw;
        }
    }

    public async Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Checking if user {UserId} has role {Role} in company {CompanyId}", guid, role, companyId);
            return await context.UserRoles.AnyAsync(ur => ur.UserId == guid && ur.CompanyId == companyId && ur.Role.Name == role, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error checking if user {UserId} has role {Role} in company {CompanyId}", guid, role, companyId);
            throw;
        }
    }
}
