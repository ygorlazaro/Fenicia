namespace Fenicia.Auth.Domains.DataCache;

using System.Text.Json;

using StackExchange.Redis;

public class RedisDataCacheService : IDataCacheService
{
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(minutes: 10);

    private readonly IDatabase db;
    private readonly ILogger<RedisDataCacheService> _logger;

    public RedisDataCacheService(IConnectionMultiplexer redis, ILogger<RedisDataCacheService> logger)
    {
        this._logger = logger;
        db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await db.StringGetAsync(key);
            if (!value.HasValue)
            {
                _logger.LogDebug("Cache miss for key: {Key}", key);
                return default;
            }

            _logger.LogDebug("Cache hit for key: {Key}", key);
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis connection error while getting key: {Key}", key);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error for key: {Key}", key);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting key: {Key}", key);
            throw;
        }
    }

    public async Task SetAsync<T>(string key, T data, TimeSpan? expiration = null)
    {
        try
        {
            var json = JsonSerializer.Serialize(data);
            await db.StringSetAsync(key, json, expiration ?? RedisDataCacheService.DefaultExpiration);
            _logger.LogDebug("Successfully cached item with key: {Key}", key);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis connection error while setting key: {Key}", key);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON serialization error for key: {Key}", key);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while setting key: {Key}", key);
            throw;
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            var result = await db.KeyDeleteAsync(key);
            _logger.LogDebug(result ? "Successfully removed cache item with key: {Key}" : "Cache item not found for removal with key: {Key}", key);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis connection error while removing key: {Key}", key);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while removing key: {Key}", key);
            throw;
        }
    }
}
