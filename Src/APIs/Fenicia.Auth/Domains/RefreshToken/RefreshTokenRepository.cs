using System.Text.Json;
using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshTokenRepository(IConnectionMultiplexer redis) : IRefreshTokenRepository
{
    private const string _redisPrefix = "refresh_token:";
    private readonly IDatabase _redisDb = redis.GetDatabase();

    public async Task AddAsync(RefreshTokenModel token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);

        var key = _redisPrefix + token.Token;
        var value = JsonSerializer.Serialize(token);

        await _redisDb.StringSetAsync(key, value, token.ExpirationDate, When.Always);
    }

    public async Task<RefreshTokenModel?> GetAsync(string token, CancellationToken cancellationToken = default)
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

    public async Task UpdateAsync(RefreshTokenModel token, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(token);

        var key = _redisPrefix + token.Token;
        var value = JsonSerializer.Serialize(token);

        await _redisDb.StringSetAsync(key, value, token.ExpirationDate, When.Always);
    }
}
