namespace Fenicia.Auth.Domains.UserRole;

using Common.Database.Contexts;

using Microsoft.EntityFrameworkCore;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AuthContext _context;
    private readonly ILogger<UserRoleRepository> _logger;

    public UserRoleRepository(AuthContext context, ILogger<UserRoleRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving roles for user {UserId}", userId);
            return await _context.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.Name).ToArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking if user {UserId} exists in company {CompanyId}", userId, companyId);
            return await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} exists in company {CompanyId}", userId, companyId);
            throw;
        }
    }

    public async Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Checking if user {UserId} has role {Role} in company {CompanyId}", guid, role, companyId);
            return await _context.UserRoles.AnyAsync(ur => ur.UserId == guid && ur.CompanyId == companyId && ur.Role.Name == role, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} has role {Role} in company {CompanyId}", guid, role, companyId);
            throw;
        }
    }
}
