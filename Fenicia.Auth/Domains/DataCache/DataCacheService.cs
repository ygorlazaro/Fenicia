namespace Fenicia.Auth.Domains.DataCache;

using System.Text.Json;

using StackExchange.Redis;

/// <summary>
///     Redis implementation of data cache service that provides caching functionality using Redis database.
///     This service handles storage, retrieval, and removal of cached items using Redis as the backend.
/// </summary>
/// <remarks>
///     The service uses JSON serialization for storing complex objects and provides default expiration time of 10 minutes.
///     All operations are performed asynchronously and include error handling with logging.
/// </remarks>
/// <param name="redis">Redis connection multiplexer instance for database operations.</param>
/// <param name="logger">Logger instance for tracking operations and errors.</param>
public class RedisDataCacheService(IConnectionMultiplexer redis, ILogger<RedisDataCacheService> logger) : IDataCacheService
{
    /// <summary>
    ///     Default expiration time for cached items (10 minutes).
    /// </summary>
    private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(minutes: 10);

    private readonly IDatabase _db = redis.GetDatabase();

    /// <summary>
    ///     Retrieves a cached item by its key.
    /// </summary>
    /// <typeparam name="T">Type of the cached item.</typeparam>
    /// <param name="key">The key of the cached item to retrieve.</param>
    /// <returns>The cached item if found; otherwise, default value of T.</returns>
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

    /// <summary>
    ///     Stores an item in the cache with the specified key.
    /// </summary>
    /// <typeparam name="T">Type of the item to cache.</typeparam>
    /// <param name="key">The key to store the item under.</param>
    /// <param name="data">The item to cache.</param>
    /// <param name="expiration">Optional expiration time. If not specified, default expiration time is used.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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

    /// <summary>
    ///     Removes an item from the cache by its key.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
