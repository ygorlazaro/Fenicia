namespace Fenicia.Auth.Domains.Security;

using Common;

public interface ISecurityService
{
    ApiResponse<string> HashPassword(string password);

    ApiResponse<bool> VerifyPassword(string password, string hashedPassword);
}
