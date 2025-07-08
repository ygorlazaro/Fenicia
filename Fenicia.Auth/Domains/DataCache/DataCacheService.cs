namespace Fenicia.Auth.Domains.DataCache;

using System.Text.Json;

using StackExchange.Redis;

public class RedisDataCacheService(IConnectionMultiplexer redis, ILogger<RedisDataCacheService> logger) : IDataCacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(minutes: 10);

    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _db.StringGetAsync(key);
            if (!value.HasValue)
            {
                logger.LogDebug(message: "Cache miss for key: {Key}", key);
                return default;
            }

            logger.LogDebug(message: "Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogError(ex, message: "Redis connection error while getting key: {Key}", key);
            throw;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, message: "JSON deserialization error for key: {Key}", key);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Unexpected error while getting key: {Key}", key);
            throw;
        }
    }

    public async Task SetAsync<T>(string key, T data, TimeSpan? expiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            await _db.StringSetAsync(key, json, expiration ?? RedisDataCacheService.DefaultExpiration);
            logger.LogDebug(message: "Successfully cached item with key: {Key}", key);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogError(ex, message: "Redis connection error while setting key: {Key}", key);
            throw;
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, message: "JSON serialization error for key: {Key}", key);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Unexpected error while setting key: {Key}", key);
            throw;
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            var result = await _db.KeyDeleteAsync(key);
            logger.LogDebug(result ? "Successfully removed cache item with key: {Key}" : "Cache item not found for removal with key: {Key}", key);
        }
        catch (RedisConnectionException ex)
        {
            logger.LogError(ex, message: "Redis connection error while removing key: {Key}", key);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Unexpected error while removing key: {Key}", key);
            throw;
        }
    }
}
