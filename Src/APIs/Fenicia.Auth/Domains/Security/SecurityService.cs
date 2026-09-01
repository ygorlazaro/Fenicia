using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Security;

public class SecurityService : ISecurityService
{
    public string Hash(string original)
    {
        if (string.IsNullOrEmpty(original))
        {
            throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(original, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new InvalidOperationException(ExceptionMessages.ErrorHashingPassword);
    }

    public bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (ArgumentException)
        {
            return false;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
