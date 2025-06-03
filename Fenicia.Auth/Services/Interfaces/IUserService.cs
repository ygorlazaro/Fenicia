using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;

namespace Fenicia.Auth.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse> GetForLoginAsync(TokenRequest request);
    bool ValidatePasswordAsync(string password, string hashedPassword);
    Task<UserResponse?> CreateNewUserAsync(UserRequest request);
    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId);
}