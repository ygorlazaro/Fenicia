using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

namespace Fenicia.Auth.Domains.Security;

/// <summary>
/// Represents a service for handling security-related operations, such as password hashing and verification.
/// </summary>
public class SecurityService : ISecurityService
{
    /// <summary>
    /// Hashes the provided original string (typically a password) using the BCrypt algorithm.
    /// </summary>
    /// <param name="original">The original string to hash.</param>
    /// <returns>The hashed string.</returns>
    /// <exception cref="BadRequestException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public string Hash(string original)
    {
        if (string.IsNullOrEmpty(original))
        {
            throw new BadRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(original, BCrypt.Net.BCrypt.GenerateSalt(12));

        return hashed ?? throw new InvalidOperationException(ExceptionMessages.ErrorHashingPassword);
    }

    /// <summary>
    /// Verifies whether the provided password matches the hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>true if the passwords match; otherwise, false.</returns>
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
