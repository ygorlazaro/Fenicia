using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Services;

/// <summary>
///     Service responsible for incrementing login attempt counters in memory cache.
///     Used to track failed login attempts for brute-force protection.
/// </summary>
/// <remarks>
///     This service is part of a brute-force protection system. When a login attempt fails,
///     this service increments the counter for that email. The counter is used by the authentication
///     logic to determine if an account should be locked out after too many failed attempts.
///     Cache entries expire after 15 minutes to allow legitimate users to retry after a cooldown period.
/// </remarks>
public class IncrementAttemptsService(IMemoryCache cache)
{
    /// <summary>
    ///     Default expiration time for login attempt entries in minutes.
    ///     After this period of inactivity, the attempt counter is automatically cleared.
    /// </summary>
    private const int ExpirationMinutes = 15;

    /// <summary>
    ///     Prefix used for all login attempt cache keys.
    /// </summary>
    private const string KeyPrefix = "login-attempt:";

    /// <summary>
    ///     Increments the login attempt counter for the specified email.
    ///     If no counter exists, initializes it to 1.
    /// </summary>
    /// <param name="email">The email address to increment attempts for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    /// <remarks>
    ///     The counter is incremented atomically by reading the current value and writing incremented value.
    ///     The cache entry is set with an absolute expiration of 15 minutes from the time of setting.
    /// </remarks>
    public Task SetKey(string email)
    {
        var key = GetKey(email);
        var current = cache.TryGetValue(key, out int count) ? count + 1 : 1;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ExpirationMinutes)
        };

        cache.Set(key, current, options);

        return Task.CompletedTask;
    }

    /// <summary>
    ///     Generates a cache key for login attempts based on email.
    /// </summary>
    /// <param name="email">The email address to generate key for.</param>
    /// <returns>Formatted cache key with lowercase email (e.g., "login-attempt:user@example.com").</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    /// <remarks>
    ///     Uses <see cref="CultureInfo.InvariantCulture" /> to ensure consistent lowercase conversion
    ///     regardless of the server's locale settings.
    /// </remarks>
    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
}