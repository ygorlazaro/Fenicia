using StackExchange.Redis;

namespace Fenicia.Auth.Domains.LoginAttempt;

public class LoginAttemptService(IConnectionMultiplexer redis)
{
    private const int _expirationMinutes = 15;
    private const string _keyPrefix = "login-attempt:";

    private readonly IDatabase _redisDb = redis.GetDatabase();

    public int GetAttempts(string email)
    {
        var key = GetKey(email);
        var value = _redisDb.StringGet(key);

        return value.HasValue ? (int)value : 0;
    }

    public async Task IncrementAsync(string email, CancellationToken ct = default)
    {
        var key = GetKey(email);
        var current = _redisDb.StringGet(key);

        var newValue = current.HasValue ? (int)current + 1 : 1;

        await _redisDb.StringSetAsync(key, newValue, TimeSpan.FromMinutes(_expirationMinutes), When.Always, CommandFlags.None);
    }

    public Task ResetAsync(string email, CancellationToken ct = default)
    {
        _redisDb.KeyDelete(GetKey(email));
        return Task.CompletedTask;
    }

    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{_keyPrefix}{email.ToLowerInvariant()}";
    }
}
