using System.Text.Json;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.InvalidateRefreshToken;

public class InvalidateRefreshTokenHandler(IConnectionMultiplexer redis)
{
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();
    
    public async Task Handler(string refreshToken, CancellationToken ct)
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

            if (tokenObj == null)
            {
                return;
            }

            tokenObj.IsActive = false;

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
        }
    }
}