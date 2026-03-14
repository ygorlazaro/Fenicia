namespace Fenicia.Auth.Domains.Security.Services;

/// <summary>
/// Service responsible for verifying passwords against BCrypt hashed passwords.
/// </summary>
public class VerifyPasswordService
{
    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="hashedPassword">The BCrypt hashed password to compare against.</param>
    /// <returns>True if password matches, false otherwise.</returns>
    /// <remarks>
    /// Returns false for null/empty inputs or any verification errors (no exceptions thrown).
    /// </remarks>
    public virtual bool Handle(string password, string hashedPassword)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
