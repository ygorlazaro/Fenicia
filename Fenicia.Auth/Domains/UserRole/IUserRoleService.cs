using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Domains.UserRole;

public interface IUserRoleService
{
    Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<CompanyResponse>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> HasRoleAsync(Guid userId, Guid companyId, string role, CancellationToken cancellationToken);
}
