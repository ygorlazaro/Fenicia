using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.Responses;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.Handlers;

/// <summary>
/// Handler responsible for invalidating (revoking) refresh tokens.
/// Sets the token's IsActive flag to false without deleting it.
/// </summary>
/// <remarks>
/// This performs a soft-delete by marking the token as inactive.
/// The token remains in Redis until its TTL expires (7 days).
/// This allows for audit trail purposes while preventing reuse.
/// </remarks>
public class InvalidateRefreshTokenHandler(IConnectionMultiplexer redis)
{
    /// <summary>
    /// Redis key prefix for refresh tokens.
    /// </summary>
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();

    /// <summary>
    /// Invalidates a refresh token by setting IsActive to false.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when refreshToken is null.</exception>
    /// <remarks>
    /// If token doesn't exist in Redis, the operation completes silently.
    /// Any exceptions during processing are silently ignored.
    /// </remarks>
    public async Task Handler(string refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
        {
            var key = RedisPrefix + refreshToken;
            var value = await this.redisDb.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                return;
            }

            var tokenObj = JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)value!);

            tokenObj?.IsActive = false;

            await this.redisDb.StringSetAsync(
                key,
                JsonSerializer.Serialize(tokenObj),
                TimeSpan.FromDays(7),
                When.Always,
                CommandFlags.None
            );
        }
        catch
        {
            // ignored
        }
    }
}
