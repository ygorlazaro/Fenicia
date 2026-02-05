using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.User;

public interface IUserRepository : IBaseRepository<UserModel>
{
    Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken);

    Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken);
}
