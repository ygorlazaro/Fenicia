using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.Responses;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.Handlers;

public class InvalidateRefreshTokenHandler(IConnectionMultiplexer redis)
{
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();

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
