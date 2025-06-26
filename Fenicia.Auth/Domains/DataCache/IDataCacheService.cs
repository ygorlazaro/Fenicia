namespace Fenicia.Auth.Domains.DataCache;

/// <summary>
/// Provides methods for managing data in a cache storage system.
/// </summary>
/// <remarks>
/// This service implements caching functionality for storing and retrieving data
/// with optional expiration time support.
/// </remarks>
public interface IDataCacheService
{
    /// <summary>
    /// Retrieves data of type T from the cache asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of data to retrieve from the cache.</typeparam>
    /// <param name="key">The unique identifier for the cached item.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// the cached data of type T if found; otherwise, null.
    /// </returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Stores data in the cache asynchronously with an optional expiration time.
    /// </summary>
    /// <typeparam name="T">The type of data to store in the cache.</typeparam>
    /// <param name="key">The unique identifier for the cached item.</param>
    /// <param name="data">The data to be stored in the cache.</param>
    /// <param name="expiration">
    /// Optional. The time span after which the cached item will expire.
    /// If null, the item will not expire.
    /// </param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T data, TimeSpan? expiration = null);

    /// <summary>
    /// Removes an item from the cache asynchronously.
    /// </summary>
    /// <param name="key">The unique identifier of the item to remove from the cache.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAsync(string key);
}
