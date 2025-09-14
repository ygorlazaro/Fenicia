namespace Fenicia.Auth.Domains.LoginAttempt;

using Microsoft.Extensions.Caching.Memory;

public class LoginAttemptService : ILoginAttemptService
{
    private readonly IMemoryCache _cache;

    public LoginAttemptService(IMemoryCache cache)
    {
        _cache = cache;
    }

    private const int ExpirationMinutes = 15;

    private const string KeyPrefix = "login-attempt:";

    public Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(_cache.TryGetValue(LoginAttemptService.GetKey(email), out int attempts) ? attempts : 0);
    }

    public Task IncrementAttemptsAsync(string email)
    {
        var key = LoginAttemptService.GetKey(email);
        var current = _cache.TryGetValue(key, out int count) ? count + 1 : 1;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LoginAttemptService.ExpirationMinutes)
        };

        _cache.Set(key, current, options);

        return Task.CompletedTask;
    }

    public Task ResetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        _cache.Remove(LoginAttemptService.GetKey(email));
        return Task.CompletedTask;
    }

    private static string GetKey(string email)
    {
        return $"{LoginAttemptService.KeyPrefix}{email.ToLower()}";
    }
}
