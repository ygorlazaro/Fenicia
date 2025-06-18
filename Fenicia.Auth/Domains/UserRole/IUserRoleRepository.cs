namespace Fenicia.Auth.Domains.UserRole;

public interface IUserRoleRepository
{
    Task<string[]> GetRolesByUserAsync(Guid userId);
    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId);
    Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role);
}
