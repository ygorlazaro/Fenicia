using System.Globalization;

using Fenicia.Auth.Domains.LoginAttempt.Queries;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Handlers;

/// <summary>
///     Handler responsible for retrieving login attempt counts from memory cache.
///     Used to track failed login attempts for account lockout protection.
/// </summary>
/// <remarks>
///     This handler is part of a brute-force protection system that tracks failed login attempts per email address.
///     It uses an in-memory cache to store attempt counts, which are automatically incremented by
///     <see cref="IncrementLoginAttemptsHandler" /> on failed logins and cleared by <see cref="ResetLoginAttemptsHandler" /> on successful logins.
///     Cache entries expire after 15 minutes of inactivity to allow legitimate users to retry.
/// </remarks>
public class GetLoginAttemptsHandler(IMemoryCache cache) : IRequestHandler<GetLoginAttemptsQuery, int>
{
    /// <summary>
    ///     Prefix used for all login attempt cache keys.
    /// </summary>
    private const string KeyPrefix = "login-attempt:";

    /// <summary>
    ///     Retrieves the current login attempt count for the specified email.
    /// </summary>
    /// <param name="email">The email address to check attempts for.</param>
    /// <returns>The number of failed login attempts, or 0 if no attempts exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when email is null.</exception>
    /// <remarks>
    ///     The email is normalized to lowercase for consistent cache key generation.
    ///     Returns 0 (not -1 or null) when no attempts exist to simplify calling code.
    /// </remarks>
    public virtual int GetAttempts(string email)
    {
        return cache.TryGetValue(GetKey(email), out int attempts) ? attempts : 0;
    }

    public Task<int> Handle(GetLoginAttemptsQuery request, CancellationToken ct)
    {
        return Task.FromResult(GetAttempts(request.Email));
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
