using Fenicia.Auth.Domains.User.Data;

namespace Fenicia.Auth.Domains.User.Logic;

public interface IUserRepository
{
    Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj, CancellationToken cancellationToken);
    UserModel Add(UserModel userRequest);
    Task<int> SaveAsync(CancellationToken cancellationToken);
    Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken);
    Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
}
