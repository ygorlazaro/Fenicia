using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Services;

/// <summary>
/// Service responsible for resetting login attempt counters in cache
/// </summary>
public class ResetAttemptsService(IMemoryCache cache)
{
    private const string KeyPrefix = "login-attempt:";

    /// <summary>
    /// Resets login attempts for the specified email address
    /// </summary>
    /// <param name="email">Email address to reset attempts for</param>
    /// <returns>A task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null</exception>
    /// <exception cref="ArgumentException">Thrown when email is empty or whitespace-only</exception>
    public Task Handle(string email)
    {
        ArgumentNullException.ThrowIfNull(email, nameof(email));

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty or whitespace-only", nameof(email));
        }

        cache.Remove(GetKey(email));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Generates a cache key for login attempts based on email
    /// </summary>
    /// <param name="email">Email address to generate key for</param>
    /// <returns>Formatted cache key</returns>
    private static string GetKey(string email)
    {
        // Normalize email to lowercase with invariant culture to ensure consistency
        return $"{KeyPrefix}{email.Trim().ToLower(CultureInfo.InvariantCulture)}";
    }
}
