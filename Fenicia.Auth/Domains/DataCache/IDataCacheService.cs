namespace Fenicia.Auth.Domains.DataCache;

public interface IDataCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T data, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
}
