using StackExchange.Redis;

namespace Fenicia.Auth.Domains.LoginAttempt.Logic;

/// <summary>
/// Manages login attempts tracking using Redis as the storage mechanism.
/// </summary>
/// <param name="redis">The Redis connection multiplexer instance for database operations.</param>
public class RedisLoginAttemptService(IConnectionMultiplexer redis) : ILoginAttemptService
{
    /// <summary>
    /// The Redis database instance used for storing login attempts.
    /// </summary>
    private readonly IDatabase _db = redis.GetDatabase();

    /// <summary>
    /// The expiration time for login attempt records (15 minutes).
    /// </summary>
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(15);

    /// <summary>
    /// Generates a Redis key for storing login attempts based on the email address.
    /// </summary>
    /// <param name="email">The email address to generate the key for.</param>
    /// <returns>A formatted Redis key string.</returns>
    private static string GetKey(string email) => $"login-attempt:{email.ToLower()}";

    /// <summary>
    /// Retrieves the number of login attempts for the specified email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of login attempts made, or 0 if no attempts are recorded.</returns>
    public async Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        var key = GetKey(email);
        var attempts = await _db.StringGetAsync(key);
        return attempts.HasValue ? (int)attempts : 0;
    }

    /// <summary>
    /// Increments the login attempt counter for the specified email address.
    /// Sets an expiration time of 15 minutes if this is the first attempt.
    /// </summary>
    /// <param name="email">The email address to increment attempts for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task IncrementAttemptsAsync(string email)
    {
        var key = GetKey(email);
        var current = await _db.StringIncrementAsync(key);
        if (current == 1)
        {
            await _db.KeyExpireAsync(key, _expiration);
        }
    }

    /// <summary>
    /// Resets the login attempts counter for the specified email address.
    /// </summary>
    /// <param name="email">The email address to reset attempts for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ResetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        await _db.KeyDeleteAsync(GetKey(email));
    }
}
