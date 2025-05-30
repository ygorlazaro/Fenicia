namespace Fenicia.Auth.Services;

public interface IUserRoleService
{
    Task<string[]> GetRolesByUserAsync(Guid userId);
}