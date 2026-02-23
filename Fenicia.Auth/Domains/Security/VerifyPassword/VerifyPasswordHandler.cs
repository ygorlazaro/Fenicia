using Fenicia.Common;

namespace Fenicia.Auth.Domains.Security.VerifyPassword;

public class VerifyPasswordHandler
{
    public virtual bool Handle(string password, string hashedPassword)
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