using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.User;

public interface IUserRepository : IBaseRepository<UserModel>
{
    Task<UserModel?> GetByEmailAsync(string email, CancellationToken ct);

    Task<bool> CheckUserExistsAsync(string email, CancellationToken ct);

    Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken ct);
}
