using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Enums;

namespace Fenicia.Auth.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(UserModel user, string[] roles, Guid companyId, List<ModuleType> modules);
}