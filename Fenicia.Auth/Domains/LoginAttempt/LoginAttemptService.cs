using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt;

public class LoginAttemptService(IMemoryCache cache) : ILoginAttemptService
{
    private const int ExpirationMinutes = 15;

    private const string KeyPrefix = "login-attempt:";

    public Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(cache.TryGetValue(GetKey(email), out int attempts) ? attempts : 0);
    }

    public Task IncrementAttemptsAsync(string email)
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

    public Task ResetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        cache.Remove(GetKey(email));

        return Task.CompletedTask;
    }

    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
}
