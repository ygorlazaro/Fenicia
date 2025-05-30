using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Requests;

namespace Fenicia.Auth.Services.Interfaces;

public interface IUserService
{
    Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj);
    Task<bool> ValidatePasswordAsync(string password, string hasedPassword);
    Task<UserModel?> CreateNewUserAsync(NewUserRequest request);
    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId);
}