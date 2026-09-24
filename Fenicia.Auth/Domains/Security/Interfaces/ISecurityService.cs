namespace Fenicia.Auth.Domains.Security.Interfaces;

/// <summary>
/// Represents a service for handling security-related operations, such as hashing and verifying passwords.
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Hashes the provided original string (e.g., a password) and returns the hashed value.
    /// </summary>
    /// <param name="original">The original string to hash.</param>
    /// <returns>The hashed string.</returns>
    string Hash(string original);

    /// <summary>
    /// Verifies that the provided password matches the hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>true if the passwords match; otherwise, false.</returns>
    bool Verify(string password, string hashedPassword);
}
