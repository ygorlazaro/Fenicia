namespace Fenicia.Auth.Domains.Security;

public interface ISecurityService
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string hashedPassword);
}
