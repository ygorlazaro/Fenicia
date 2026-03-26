using System.Security.Cryptography;
using System.Text.Json;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.Handlers;

/// <summary>
///     Handler responsible for generating secure refresh tokens.
///     Creates cryptographically strong refresh tokens stored in Redis.
/// </summary>
/// <remarks>
///     Refresh tokens are used for session management and token renewal.
///     Tokens are:
///     - 32 bytes of cryptographically secure random data (Base64 encoded)
///     - Valid for 7 days
///     - Stored in Redis with automatic expiration
///     - Associated with a specific user ID
/// </remarks>
public class GenerateRefreshTokenHandler(IConnectionMultiplexer redis)
{
    /// <summary>
    ///     Redis key prefix for refresh tokens.
    /// </summary>
    private const string RedisPrefix = "refresh_token:";

    private readonly IDatabase redisDb = redis.GetDatabase();

    /// <summary>
    ///     Generates a new cryptographically secure refresh token for the specified user.
    /// </summary>
    /// <param name="userId">The user ID to associate with the token.</param>
    /// <returns>A Base64-encoded refresh token string.</returns>
    /// <remarks>
    ///     Uses <see cref="RandomNumberGenerator" /> for cryptographically secure random bytes.
    ///     Token is automatically saved to Redis with 7-day expiration.
    /// </remarks>
    public string Handle(Guid userId)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var stringToken = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshTokenModel(stringToken, DateTime.UtcNow.AddDays(7), userId);

        Add(refreshToken);

        return refreshToken.Token;
    }

    /// <summary>
    ///     Saves the refresh token to Redis with 7-day expiration.
    /// </summary>
    /// <param name="refreshToken">The refresh token to save.</param>
    /// <exception cref="ArgumentNullException">Thrown when refreshToken is null.</exception>
    private void Add(RefreshTokenModel refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var key = RedisPrefix + refreshToken.Token;
        var value = JsonSerializer.Serialize(refreshToken);

        redisDb.StringSet(key, value, TimeSpan.FromDays(7), When.Always, CommandFlags.None);
    }
}
