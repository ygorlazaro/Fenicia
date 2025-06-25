using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt;

public class LoginAttemptService(IMemoryCache cache) : ILoginAttemptService
{
    private const int ExpirationMinutes = 15;
    private const string KeyPrefix = "login-attempt:";

    public Task<int> GetAttemptsAsync(string email)
    {
        if (cache.TryGetValue(GetKey(email), out int attempts))
        {
            return Task.FromResult(attempts);
        }

        return Task.FromResult(0);
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

    public Task ResetAttemptsAsync(string email)
    {
        cache.Remove(GetKey(email));
        return Task.CompletedTask;
    }

    private static string GetKey(string email) => $"{KeyPrefix}{email.ToLower()}";
}
