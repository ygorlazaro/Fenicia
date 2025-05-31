using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Requests;

namespace Fenicia.Auth.Services.Interfaces;

public interface IUserService
{
    Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj);
    bool ValidatePasswordAsync(string password, string hashedPassword);
    Task<UserModel?> CreateNewUserAsync(NewUserRequest request);
    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId);
}