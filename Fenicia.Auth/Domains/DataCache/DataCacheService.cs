using System.Text.Json;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.DataCache;

public class RedisDataCacheService(IConnectionMultiplexer redis) : IDataCacheService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(10);

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task SetAsync<T>(string key, T data, TimeSpan? expiration = null)
    {
        var json = JsonSerializer.Serialize(data);
        await _db.StringSetAsync(key, json, expiration ?? DefaultExpiration);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
