using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt;

public class LoginAttemptService(IMemoryCache cache)
{
    private const int _expirationMinutes = 15;
    private const string _keyPrefix = "login-attempt:";

    public int GetAttempts(string email)
    {
        return cache.TryGetValue(GetKey(email), out int attempts) ? attempts : 0;
    }

    public Task IncrementAsync(string email, CancellationToken ct = default)
    {
        var key = GetKey(email);
        var current = cache.TryGetValue(key, out int count) ? count + 1 : 1;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_expirationMinutes)
        };

        cache.Set(key, current, options);

        return Task.CompletedTask;
    }

    public Task ResetAsync(string email, CancellationToken ct = default)
    {
        cache.Remove(GetKey(email));
        return Task.CompletedTask;
    }

    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{_keyPrefix}{email.ToLowerInvariant()}";
    }
}
