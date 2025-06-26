using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Logic;

/// <summary>
/// Service responsible for managing login attempts and their tracking.
/// </summary>
public class LoginAttemptService(IMemoryCache cache) : ILoginAttemptService
{
    /// <summary>
    /// The expiration time in minutes for login attempt records.
    /// </summary>
    private const int ExpirationMinutes = 15;

    /// <summary>
    /// The prefix used for cache keys related to login attempts.
    /// </summary>
    private const string KeyPrefix = "login-attempt:";

    /// <summary>
    /// Gets the number of login attempts for the specified email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The number of login attempts made by the email address.</returns>
    public Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(cache.TryGetValue(GetKey(email), out int attempts) ? attempts : 0);
    }

    /// <summary>
    /// Increments the number of login attempts for the specified email address.
    /// </summary>
    /// <param name="email">The email address to increment attempts for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task IncrementAttemptsAsync(string email)
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
    /// Resets the login attempts counter for the specified email address.
    /// </summary>
    /// <param name="email">The email address to reset attempts for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task ResetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        cache.Remove(GetKey(email));
        return Task.CompletedTask;
    }

    /// <summary>
    /// Generates a cache key for the specified email address.
    /// </summary>
    /// <param name="email">The email address to generate a key for.</param>
    /// <returns>A cache key string.</returns>
    private static string GetKey(string email) => $"{KeyPrefix}{email.ToLower()}";
}
