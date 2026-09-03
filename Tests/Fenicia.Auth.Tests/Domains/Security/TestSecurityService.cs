using Fenicia.Auth.Domains.Security;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Tests.Domains.Security;

public class TestSecurityService : SecurityService
{
    public new string Hash(string original)
    {
        if (string.IsNullOrEmpty(original))
        {
            throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(original, BCrypt.Net.BCrypt.GenerateSalt(4));

        return hashed ?? throw new InvalidOperationException(ExceptionMessages.ErrorHashingPassword);
    }
}