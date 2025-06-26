using Fenicia.Common;

namespace Fenicia.Auth.Domains.Security.Logic;

public interface ISecurityService
{
    ApiResponse<string> HashPassword(string password);
    ApiResponse<bool> VerifyPassword(string password, string hashedPassword);
}
