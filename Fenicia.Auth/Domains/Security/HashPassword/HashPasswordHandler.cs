using Fenicia.Common;

namespace Fenicia.Auth.Domains.Security.HashPassword;

public class HashPasswordHandler
{
    public string Handle(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException(TextConstants.InvalidPasswordMessage);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new Exception("Error hashing password");
    }

}