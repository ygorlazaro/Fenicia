namespace Fenicia.Auth.Domains.Security.Interfaces;

public interface ISecurityService
{
    string Hash(string original);

    bool Verify(string password, string hashedPassword);
}