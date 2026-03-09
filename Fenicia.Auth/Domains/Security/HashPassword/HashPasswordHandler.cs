using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Security.HashPassword;

public class HashPasswordHandler
{
    public virtual string Handle(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new Exception(ExceptionMessages.ErrorHashingPassword);
    }
}
