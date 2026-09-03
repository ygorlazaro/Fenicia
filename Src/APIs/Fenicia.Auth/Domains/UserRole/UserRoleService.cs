using Fenicia.Auth.Domains.UserRole.DTOs;
using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(IUserRoleRepository userRoleRepository) : IUserRoleService
{
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRoles = await userRoleRepository.GetCompaniesByUserAsync(userId, cancellationToken);

        return [.. userRoles.Select(ur => ur.MapToUserRoleResponse())];
    }

    public async Task<List<GetUserCompaniesResponse>> GetUserCompaniesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRoles = await userRoleRepository.GetUserCompaniesAsync(userId, cancellationToken);

        return [.. userRoles.Select(ur => ur.MapToGetUserCompaniesResponse())];
    }

    public Task<List<UserRoleModel>> GetUserRolesAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        return userRoleRepository.GetUserRolesAsync(userId, page, perPage, cancellationToken);
    }

    public Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return userRoleRepository.CountUserRolesAsync(userId, cancellationToken);
    }

    public Task<UserRoleModel?> GetUserRoleAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return userRoleRepository.GetUserRoleAsync(userId, companyId, cancellationToken);
    }

    public Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return userRoleRepository.IsAdminAsync(userId, companyId, cancellationToken);
    }

    public Task<bool> AnyIdAndCompanyAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return userRoleRepository.AnyIdAndCompanyAsync(userId, companyId, cancellationToken);
    }

    public Task<bool> HasRoleAsync(
        Guid userId,
        Guid companyId,
        string role,
        CancellationToken cancellationToken = default)
    {
        return userRoleRepository.HasRoleAsync(userId, companyId, role, cancellationToken);
    }

    public Task InsertRangeAsync(List<UserRoleModel> userRoles, CancellationToken cancellationToken = default)
    {
        return userRoleRepository.InsertRangeAsync(userRoles, cancellationToken);
    }

    public Task<UserRoleModel> InsertAsync(UserRoleModel userRole, CancellationToken cancellationToken = default)
    {
        return userRoleRepository.InsertAsync(userRole, cancellationToken);
    }

    public async Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        await userRoleRepository.DeleteAsync(roleId, cancellationToken);
    }

    public Task<List<UserRoleModel>> GetUserRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return userRoleRepository.Query()
            .Include(x => x.Role)
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<UserRoleModel>> GetUserRoleModelsByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return userRoleRepository.GetCompaniesByUserAsync(userId, cancellationToken);
    }
}