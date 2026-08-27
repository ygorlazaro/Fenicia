using Fenicia.Auth.Domains.LoginAttempt.DTOs.Commands;
using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt;

public class LoginAttemptService(IMemoryCache cache)
{
    private const int ExpirationMinutes = 15;
    private const string KeyPrefix = "login-attempt:";

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
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ExpirationMinutes)
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

        return $"{KeyPrefix}{email.ToLowerInvariant()}";
    }
}
