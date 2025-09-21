namespace Fenicia.Auth.Domains.UserRole;

using Fenicia.Common.Database.Models.Auth;

public interface IUserRoleRepository
{
    Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);

    Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken);

    Task<List<UserRoleModel>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken);
}
