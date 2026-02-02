using System.Globalization;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.LoginAttempt;

public class RedisLoginAttemptService(IConnectionMultiplexer redis) : ILoginAttemptService
{
    private readonly IDatabase db = redis.GetDatabase();

    private readonly TimeSpan expiration = TimeSpan.FromMinutes(minutes: 15);

    public async Task<int> GetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        var key = RedisLoginAttemptService.GetKey(email);
        var attempts = await this.db.StringGetAsync(key);

        return attempts.HasValue ? (int)attempts : 0;
    }

    public async Task IncrementAttemptsAsync(string email)
    {
        var key = RedisLoginAttemptService.GetKey(email);
        var current = await this.db.StringIncrementAsync(key);

        if (current == 1)
        {
            await this.db.KeyExpireAsync(key, this.expiration);
        }
    }

    public async Task ResetAttemptsAsync(string email, CancellationToken cancellationToken)
    {
        await this.db.KeyDeleteAsync(RedisLoginAttemptService.GetKey(email));
    }

    private static string GetKey(string email)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "login-attempt:{0}",
            email.ToLower(CultureInfo.InvariantCulture));
    }
}
