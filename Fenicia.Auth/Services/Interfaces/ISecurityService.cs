using Fenicia.Common;

namespace Fenicia.Auth.Services.Interfaces;

public interface ISecurityService
{
    ServiceResponse<string> HashPassword(string password);
    ServiceResponse<bool> VerifyPassword(string password, string hashedPassword);
}