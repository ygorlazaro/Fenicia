namespace Fenicia.Auth.Domains.LoginAttempt;

using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

public class LoginAttemptService : ILoginAttemptService
{
    private const int ExpirationMinutes = 15;

    private const string KeyPrefix = "login-attempt:";

    private readonly IMemoryCache cache;

    public LoginAttemptService(IMemoryCache cache)
    {
        this.cache = cache;
    }

    public Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        return Task.FromResult(this.cache.TryGetValue(LoginAttemptService.GetKey(email), out int attempts) ? attempts : 0);
    }

    public Task IncrementAttemptsAsync(string email)
    {
        var key = LoginAttemptService.GetKey(email);
        var current = this.cache.TryGetValue(key, out int count) ? count + 1 : 1;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(LoginAttemptService.ExpirationMinutes)
        };

        this.cache.Set(key, current, options);

        return Task.CompletedTask;
    }

    public Task ResetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        this.cache.Remove(LoginAttemptService.GetKey(email));
        return Task.CompletedTask;
    }

    private static string GetKey(string email)
    {
        return $"{LoginAttemptService.KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
}
