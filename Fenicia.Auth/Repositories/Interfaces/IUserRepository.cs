using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj);
    UserModel Add(UserModel userRequest);
    Task<int> SaveAsync();
    Task<bool> CheckUserExistsAsync(string email);
}

