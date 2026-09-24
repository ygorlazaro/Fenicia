using Fenicia.Auth.Domains.LoginAttempt.Interfaces;
using StackExchange.Redis;

namespace Fenicia.Auth.Domains.LoginAttempt;

/// <summary>
/// Represents a service for managing login attempts.
/// </summary>
/// <param name="redis">The Redis connection multiplexer.</param>
public class LoginAttemptService(IConnectionMultiplexer redis) : ILoginAttemptService
{
    /// <summary>
    /// The expiration time for login attempts in minutes.
    /// </summary>
    private const int _expirationMinutes = 15;

    /// <summary>
    /// The prefix for the Redis key used to store login attempt information.
    /// </summary>
    private const string _keyPrefix = "login-attempt:";

    /// <summary>
    /// The Redis database instance used for storing login attempt information.
    /// </summary>
    private readonly IDatabase _redisDb = redis.GetDatabase();

    /// <summary>
    /// Gets the number of login attempts for a specific email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>The number of login attempts.</returns>
    public int GetAttempts(string email)
    {
        var key = GetKey(email);
        var value = _redisDb.StringGet(key);

        return value.HasValue ? (int)value : 0;
    }

    /// <summary>
    /// Increments the number of login attempts for a specific email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task IncrementAsync(string email)
    {
        var key = GetKey(email);
        var current = _redisDb.StringGet(key);

        var newValue = current.HasValue ? (int)current + 1 : 1;

        await _redisDb.StringSetAsync(
            key,
            newValue,
            TimeSpan.FromMinutes(_expirationMinutes),
            When.Always,
            CommandFlags.None);
    }

    /// <summary>
    /// Resets the number of login attempts for a specific email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    public void Reset(string email)
    {
        _redisDb.KeyDelete(GetKey(email));
    }

    /// <summary>
    /// Generates the Redis key for storing login attempt information based on the provided email address.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <returns>The Redis key.</returns>
    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{_keyPrefix}{email.ToLowerInvariant()}";
    }
}
