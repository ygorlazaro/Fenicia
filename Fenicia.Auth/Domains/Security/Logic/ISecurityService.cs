namespace Fenicia.Auth.Domains.Security.Logic;

using Common;

/// <summary>
///     Provides security-related services for password hashing and verification.
/// </summary>
public interface ISecurityService
{
    /// <summary>
    ///     Hashes a plain text password using a secure hashing algorithm.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <returns>An ApiResponse containing the hashed password string if successful, or error details if failed.</returns>
    ApiResponse<string> HashPassword(string password);

    /// <summary>
    ///     Verifies if a plain text password matches its hashed version.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>An ApiResponse containing true if passwords match, false otherwise, or error details if verification failed.</returns>
    ApiResponse<bool> VerifyPassword(string password, string hashedPassword);
}
