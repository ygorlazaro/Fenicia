using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Security;

public static class HashStringExtensions
{
    public static string Hash(this string original)
    {
        if (string.IsNullOrEmpty(original))
        {
            throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(original, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new Exception(ExceptionMessages.ErrorHashingPassword);
    }
}
