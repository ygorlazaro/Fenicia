using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.UserRole;

public interface IUserRoleRepository
{
    Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken ct);

    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken ct);

    Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken ct);

    Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken ct);

    void Add(UserRoleModel userRole);
}