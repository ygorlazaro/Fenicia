using Fenicia.Auth.Domains.UserRole.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Auth.Domains.UserRole;

public class UserRoleService(IUserRoleRepository repository) : IUserRoleService
{
    public async Task<List<UserRoleResponse>> GetCompaniesByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRoles = await repository.GetCompaniesByUserAsync(userId, cancellationToken);

        return [.. userRoles.Select(MapToUserRoleResponse)];
    }

    public async Task<List<UserCompanyResponse>> GetUserCompaniesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var userRoles = await repository.GetUserCompaniesAsync(userId, cancellationToken);

        return [.. userRoles.Select(MapToGetUserCompaniesResponse)];
    }

    public Task<List<UserRoleModel>> GetUserRolesAsync(
        Guid userId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        return repository.GetUserRolesAsync(userId, page, perPage, cancellationToken);
    }

    public Task<int> CountUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return repository.CountUserRolesAsync(userId, cancellationToken);
    }

    public Task<UserRoleModel?> GetUserRoleAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetUserRoleAsync(userId, companyId, cancellationToken);
    }

    public Task<bool> IsAdminAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        return repository.IsAdminAsync(userId, companyId, cancellationToken);
    }

    public Task<bool> AnyIdAndCompanyAsync(
        Guid userId,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return repository.AnyIdAndCompanyAsync(userId, companyId, cancellationToken);
    }

    public Task<bool> HasRoleAsync(
        Guid userId,
        Guid companyId,
        string role,
        CancellationToken cancellationToken = default)
    {
        return repository.HasRoleAsync(userId, companyId, role, cancellationToken);
    }

    public Task InsertRangeAsync(List<UserRoleModel> userRoles, CancellationToken cancellationToken = default)
    {
        return repository.InsertRangeAsync(userRoles, cancellationToken);
    }

    public Task<UserRoleModel> InsertAsync(UserRoleModel userRole, CancellationToken cancellationToken = default)
    {
        return repository.InsertAsync(userRole, cancellationToken);
    }

    public async Task DeleteAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(roleId, cancellationToken);
    }

    public Task<List<UserRoleModel>> GetUserRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetUserRolesByIdAsync(userId, cancellationToken);
    }

    public Task<List<UserRoleModel>> GetUserRoleModelsByUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return repository.GetCompaniesByUserAsync(userId, cancellationToken);
    }

    private static UserRoleResponse MapToUserRoleResponse(UserRoleModel userRole)
    {
        return new UserRoleResponse(
            userRole.Id,
            userRole.Role.Name,
            new CompanyResponse(
                userRole.Company.Id,
                userRole.Company.Name,
                userRole.Company.Cnpj));
    }

    private static UserCompanyResponse MapToGetUserCompaniesResponse(UserRoleModel userRole)
    {
        return new UserCompanyResponse(
            userRole.Company.Id,
            userRole.Role.Name,
            userRole.CompanyId,
            userRole.Company.Name,
            userRole.Company.Cnpj);
    }
}
