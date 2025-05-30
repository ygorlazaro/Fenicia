namespace Fenicia.Auth.Repositories;

public interface IUserRoleRepository
{
    Task<string[]> GetRolesByUserAsync(Guid userId);
}