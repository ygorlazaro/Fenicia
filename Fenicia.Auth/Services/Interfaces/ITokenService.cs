using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserModel user, string[] roles, Guid companyId);
}