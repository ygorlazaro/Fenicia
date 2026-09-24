using System.Text.Json;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken;

/// <summary>
/// Represents a repository for managing refresh tokens using Redis as the underlying storage mechanism.
/// </summary>
/// <param name="redis">The Redis connection multiplexer used to interact with the Redis database.</param>
public class RefreshTokenRepository(IConnectionMultiplexer redis) : IRefreshTokenRepository
{
    /// <summary>
    /// The prefix used for keys in Redis to store refresh tokens, ensuring that all refresh token entries are easily identifiable and grouped together in the Redis database.
    /// </summary>
    private const string _redisPrefix = "refresh_token:";

    /// <summary>
    /// The Redis database instance obtained from the provided Redis connection multiplexer, used for performing operations such as adding, retrieving, and updating refresh tokens in the Redis store.
    /// </summary>
    private readonly IDatabase _redisDb = redis.GetDatabase();

    /// <summary>
    /// Adds a new refresh token to the Redis repository, serializing the token model to JSON and storing it with an expiration date, ensuring that the token is available for retrieval until it expires.
    /// </summary>
    /// <param name="token">The refresh token model to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task AddAsync(RefreshTokenModel token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var key = _redisPrefix + token.Token;
        var value = JsonSerializer.Serialize(token);

        await _redisDb.StringSetAsync(key, value, token.ExpirationDate, When.Always);
    }

    /// <summary>
    /// Retrieves a refresh token from the Redis repository based on the provided token string. If the token is found, it is deserialized from JSON back into a RefreshTokenModel object; if not found or if an error occurs during retrieval or deserialization, null is returned.
    /// </summary>
    /// <param name="token">The refresh token string to retrieve.</param>
    /// <returns>The refresh token model if found; otherwise, null.</returns>
    public async Task<RefreshTokenModel?> GetAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var key = _redisPrefix + token;
            var value = await _redisDb.StringGetAsync(key);

            return value.IsNullOrEmpty ? null : JsonSerializer.Deserialize<RefreshTokenModel>(value.ToString());
        }
        catch (RedisException)
        {
            return null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// Updates an existing refresh token in the Redis repository, serializing the token model to JSON and storing it with an expiration date, ensuring that the token is available for retrieval until it expires.
    /// </summary>
    /// <param name="token">The refresh token model to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateAsync(RefreshTokenModel token)
    {
        ArgumentNullException.ThrowIfNull(token);

        var key = _redisPrefix + token.Token;
        var value = JsonSerializer.Serialize(token);

        await _redisDb.StringSetAsync(key, value, token.ExpirationDate, When.Always);
    }
}
