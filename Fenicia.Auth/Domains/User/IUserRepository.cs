namespace Fenicia.Auth.Domains.User;

public interface IUserRepository
{
    Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj);
    UserModel Add(UserModel userRequest);
    Task<int> SaveAsync();
    Task<bool> CheckUserExistsAsync(string email);
    Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId);
    Task<Guid?> GetUserIdFromEmailAsync(string email);
    Task<UserModel?> GetByIdAsync(Guid userId);
}
