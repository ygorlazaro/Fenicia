namespace Fenicia.Auth.Domains.Security;

public static class VerifyPasswordExtensions
{
    public static bool Verify(this string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
#pragma warning disable CA1031
        catch (Exception)
        {
#pragma warning restore CA1031
            return false;
        }
    }
}
