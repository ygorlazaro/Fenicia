namespace Fenicia.Auth.Services.Interfaces;

public interface ISecurityService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}