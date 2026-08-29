using System.Text.Json;
using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshTokenRepository(IConnectionMultiplexer redis) : IRefreshTokenRepository
{
    private const string _redisPrefix = "refresh_token:";
    private readonly IDatabase _redisDb = redis.GetDatabase();

    public async Task AddAsync(RefreshTokenModel token, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(token);

        var key = _redisPrefix + token.Token;
        var value = JsonSerializer.Serialize(token);

        await _redisDb.StringSetAsync(key, value, token.ExpirationDate, When.Always, CommandFlags.None);
    }

    public async Task<RefreshTokenModel?> GetAsync(string token, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        try
        {
            var key = _redisPrefix + token;
            var value = await _redisDb.StringGetAsync(key, CommandFlags.None);

            if (value.IsNullOrEmpty)
            {
                return null;
            }

            return JsonSerializer.Deserialize<RefreshTokenModel>(value.ToString());
        }
#pragma warning disable CA1031
        catch
        {
            return null;
        }
#pragma warning restore CA1031
    }

    public async Task UpdateAsync(RefreshTokenModel token, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(token);

        var key = _redisPrefix + token.Token;
        var value = JsonSerializer.Serialize(token);

        await _redisDb.StringSetAsync(key, value, token.ExpirationDate, When.Always, CommandFlags.None);
    }
}
