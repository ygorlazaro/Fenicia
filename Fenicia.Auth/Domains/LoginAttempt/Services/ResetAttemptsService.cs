using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Services;

/// <summary>
///     Service responsible for resetting login attempt counters in memory cache.
///     Called after successful login to clear failed attempt counts.
/// </summary>
/// <remarks>
///     This service is part of a brute-force protection system. When a user successfully logs in,
///     this service removes the attempt counter from the cache, allowing the user to start fresh.
///     This is typically called as part of the successful authentication flow.
/// </remarks>
public class ResetAttemptsService(IMemoryCache cache)
{
    /// <summary>
    ///     Prefix used for all login attempt cache keys.
    /// </summary>
    private const string KeyPrefix = "login-attempt:";

    /// <summary>
    ///     Resets login attempts for the specified email address.
    ///     Removes the attempt counter from cache, allowing the user to start fresh.
    /// </summary>
    /// <param name="email">Email address to reset attempts for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    /// <exception cref="ArgumentException">Thrown when email is empty or whitespace-only.</exception>
    /// <remarks>
    ///     The email is trimmed and normalized to lowercase before generating the cache key.
    ///     This method safely handles cases where no attempts exist (no-op).
    /// </remarks>
    public Task Handle(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty or whitespace-only");
        }

        cache.Remove(GetKey(email));
        return Task.CompletedTask;
    }

    /// <summary>
    ///     Generates a cache key for login attempts based on email.
    /// </summary>
    /// <param name="email">Email address to generate key for.</param>
    /// <returns>Formatted cache key with lowercase, trimmed email.</returns>
    /// <remarks>
    ///     Normalizes email to lowercase with invariant culture to ensure consistency.
    ///     Also trims whitespace from the email before processing.
    /// </remarks>
    private static string GetKey(string email)
    {
        // Normalize email to lowercase with invariant culture to ensure consistency
        return $"{KeyPrefix}{email.Trim().ToLower(CultureInfo.InvariantCulture)}";
    }
}