namespace Fenicia.Auth.Domains.LoginAttempt.Logic;

using StackExchange.Redis;

public class RedisLoginAttemptService(IConnectionMultiplexer redis) : ILoginAttemptService
{
    private readonly IDatabase _db = redis.GetDatabase();

    private readonly TimeSpan _expiration = TimeSpan.FromMinutes(minutes: 15);

    public async Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        var key = RedisLoginAttemptService.GetKey(email);
        var attempts = await _db.StringGetAsync(key);
        return attempts.HasValue ? (int)attempts : 0;
    }

    public async Task IncrementAttemptsAsync(string email)
    {
        var key = RedisLoginAttemptService.GetKey(email);
        var current = await _db.StringIncrementAsync(key);
        if (current == 1)
        {
            await _db.KeyExpireAsync(key, _expiration);
        }
    }

    public async Task ResetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        await _db.KeyDeleteAsync(RedisLoginAttemptService.GetKey(email));
    }

    private static string GetKey(string email)
    {
        return $"login-attempt:{email.ToLower()}";
    }
}
