using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

/// <summary>
/// Repository implementation for managing user roles in the authentication domain.
/// </summary>
/// <param name="context">The database context.</param>
public class UserRoleRepository(DbContext context) : Repository<UserRoleModel>(context), IUserRoleRepository
{
    /// <summary>
    /// Gets all companies associated with a user including their roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    public Task<List<UserRoleModel>> GetCompaniesByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets all companies associated with a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    public Task<List<UserRoleModel>> GetUserCompaniesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a paginated list of user roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    public Task<List<UserRoleModel>> GetUserRolesAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(ur => ur.UserId == userId && ur.Company.IsActive)
            .Include(ur => ur.Role)
            .Include(ur => ur.Company)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets the total count of user roles for a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the count.</returns>
    public Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet.CountAsync(ur => ur.UserId == userId && ur.Company.IsActive, cancellationToken);
    }

    /// <summary>
    /// Gets a user role by user ID and company ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the user role model if found.</returns>
    public Task<UserRoleModel?> GetUserRoleAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
    }

    /// <summary>
    /// Checks if a user is an admin for a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user is admin.</returns>
    public async Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        var isAdmin = await DbSet
            .AnyAsync(
                ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == "Admin",
                cancellationToken);

        if (isAdmin)
        {
            return true;
        }

        var isGod = await HasRoleAsync(userId, companyId, "God", cancellationToken);

        return isGod;
    }

    /// <summary>
    /// Checks if a user has any role for a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user has any role.</returns>
    public Task<bool> AnyIdAndCompanyAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(ur => ur.UserId == userId && ur.CompanyId == companyId, cancellationToken);
    }

    /// <summary>
    /// Checks if a user has a specific role for a company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="role">The role name to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with true if user has the role.</returns>
    public Task<bool> HasRoleAsync(
        Guid userId,
        Guid companyId,
        string role,
        CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(
            ur => ur.UserId == userId && ur.CompanyId == companyId && ur.Role.Name == role,
            cancellationToken);
    }

    /// <summary>
    /// Gets all roles for a user by user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with a list of user role models.</returns>
    public Task<List<UserRoleModel>> GetUserRolesByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var query = from ur in DbSet
                    where ur.UserId == userId
                    select ur;

        return query
            .Include(x => x.Role)
            .ToListAsync(cancellationToken);
    }
}
