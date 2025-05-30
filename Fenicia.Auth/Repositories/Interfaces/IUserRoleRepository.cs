namespace Fenicia.Auth.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task<string[]> GetRolesByUserAsync(Guid userId);
    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId);
}