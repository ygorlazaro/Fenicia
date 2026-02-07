using Fenicia.Common.Data.Responses.Auth;

namespace Fenicia.Auth.Domains.UserRole;

public interface IUserRoleService
{
    Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken ct);

    Task<List<CompanyResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken ct);

    Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken ct);
}