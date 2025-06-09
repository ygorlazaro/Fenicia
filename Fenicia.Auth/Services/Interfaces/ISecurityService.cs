using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISecurityService
{
    ApiResponse<string> HashPassword(string password);
    ApiResponse<bool> VerifyPassword(string password, string hashedPassword);
}
