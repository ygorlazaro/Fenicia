namespace Fenicia.Auth.Domains.LoginAttempt;

using StackExchange.Redis;

public class RedisLoginAttemptService(IConnectionMultiplexer redis) : ILoginAttemptService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(15);

    private static string GetKey(string email) => $"login-attempt:{email.ToLower()}";

    public async Task<int> GetAttemptsAsync(string email)
    {
        var key = GetKey(email);
        var attempts = await _db.StringGetAsync(key);
        return attempts.HasValue ? (int)attempts : 0;
    }

    public async Task IncrementAttemptsAsync(string email)
    {
        var key = GetKey(email);
        var current = await _db.StringIncrementAsync(key);
        if (current == 1)
        {
            await _db.KeyExpireAsync(key, _expiration);
        }
    }

    public async Task ResetAttemptsAsync(string email)
    {
        await _db.KeyDeleteAsync(GetKey(email));
    }
}
