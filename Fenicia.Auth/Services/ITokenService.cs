using Fenicia.Auth.Contexts.Models;

namespace Fenicia.Auth.Services;

public interface ITokenService
{
    string GenerateToken(UserModel user, string[] roles, Guid companyId);
}