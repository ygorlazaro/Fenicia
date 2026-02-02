using Fenicia.Common.Database.Models.Auth;

namespace Fenicia.Auth.Domains.User;

public interface IUserRepository
{
    Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    UserModel Add(UserModel userRequest);

    Task<int> SaveAsync(CancellationToken cancellationToken);

    Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken);

    Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);

    Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken);

    Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
}
