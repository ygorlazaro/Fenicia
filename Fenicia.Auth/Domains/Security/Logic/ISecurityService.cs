namespace Fenicia.Auth.Domains.Security.Logic;

using Common;

public interface ISecurityService
{
    ApiResponse<string> HashPassword(string password);

    ApiResponse<bool> VerifyPassword(string password, string hashedPassword);
}
