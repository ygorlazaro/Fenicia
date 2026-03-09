using Fenicia.Common.Exceptions;

namespace Fenicia.Auth.Domains.Security.HashPassword;

public class HashPasswordHandler
{
    public virtual string Handle(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new InvalidRequestException("Password cannot be null or empty");
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new Exception("Error hashing password");
    }
}
