using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories;

public interface IUserRepository
{
    Task<UserModel?> GetByEmailAsync(string email);
    UserModel Add(UserModel userRequest);
    Task<int> SaveAsync();
    Task<bool> CheckUserExistsAsync(string email);
}

