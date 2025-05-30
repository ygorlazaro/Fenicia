namespace Fenicia.Auth.Services.Interfaces;

public interface IUserRoleService
{
    Task<string[]> GetRolesByUserAsync(Guid userId);
}