using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Services;

/// <summary>
/// Service responsible for retrieving login attempt counts from memory cache.
/// Used to track failed login attempts for account lockout protection.
/// </summary>
/// <remarks>
/// This service is part of a brute-force protection system that tracks failed login attempts per email address.
/// It uses an in-memory cache to store attempt counts, which are automatically incremented by
/// <see cref="IncrementAttemptsService"/> on failed logins and cleared by <see cref="ResetAttemptsService"/> on successful logins.
/// Cache entries expire after 15 minutes of inactivity to allow legitimate users to retry.
/// </remarks>
public class LoginAttemptService(IMemoryCache cache)
{
    /// <summary>
    /// Prefix used for all login attempt cache keys.
    /// </summary>
    private const string KeyPrefix = "login-attempt:";

    /// <summary>
    /// Retrieves the current login attempt count for the specified email.
    /// </summary>
    /// <param name="email">The email address to check attempts for.</param>
    /// <returns>The number of failed login attempts, or 0 if no attempts exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    /// <remarks>
    /// The email is normalized to lowercase for consistent cache key generation.
    /// Returns 0 (not -1 or null) when no attempts exist to simplify calling code.
    /// </remarks>
    public virtual int Handle(string email)
    {
        return cache.TryGetValue(GetKey(email), out int attempts) ? attempts : 0;
    }

    /// <summary>
    /// Generates a cache key for login attempts based on email.
    /// </summary>
    /// <param name="email">The email address to generate key for.</param>
    /// <returns>Formatted cache key with lowercase email (e.g., "login-attempt:user@example.com").</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    /// <remarks>
    /// Uses <see cref="CultureInfo.InvariantCulture"/> to ensure consistent lowercase conversion
    /// regardless of the server's locale settings.
    /// </remarks>
    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
}
