using Fenicia.Common;

namespace Fenicia.Auth.Domains.Security;

public class SecurityService : ISecurityService
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException(TextConstants.InvalidPasswordMessage);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new Exception("Error hashing password");
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            return string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword)
                ? throw new ArgumentException(TextConstants.InvalidPasswordMessage)
                : BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception)
        {
            return false;
        }
    }
}