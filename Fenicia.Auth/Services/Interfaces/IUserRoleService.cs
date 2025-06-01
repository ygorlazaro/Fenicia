namespace Fenicia.Auth.Services.Interfaces;

public interface IUserRoleService
{
    Task<string[]> GetRolesByUserAsync(Guid userId);
    Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role);
}