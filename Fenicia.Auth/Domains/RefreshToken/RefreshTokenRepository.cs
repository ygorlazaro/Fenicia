namespace Fenicia.Auth.Domains.RefreshToken;

using System.Text.Json;

using StackExchange.Redis;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb;

    public RefreshTokenRepository(IConnectionMultiplexer redis)
    {
        this.redisDb = redis.GetDatabase();
    }

    public void Add(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        var key = RedisPrefix + refreshToken.Token;
        var value = JsonSerializer.Serialize(refreshToken);
        this.redisDb.StringSet(key, value, TimeSpan.FromDays(7));
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        var key = RedisPrefix + refreshToken;
        var value = await this.redisDb.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return false;
        }

        var tokenObj = JsonSerializer.Deserialize<RefreshToken>(value!);
        return tokenObj != null && tokenObj.UserId == userId && tokenObj.IsActive && tokenObj.ExpirationDate > DateTime.UtcNow;
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        var key = RedisPrefix + refreshToken;
        var value = await this.redisDb.StringGetAsync(key);
        if (value.IsNullOrEmpty)
        {
            return;
        }

        var tokenObj = JsonSerializer.Deserialize<RefreshToken>(value!);
        if (tokenObj == null)
        {
            return;
        }

        tokenObj.IsActive = false;
        await this.redisDb.StringSetAsync(key, JsonSerializer.Serialize(tokenObj), TimeSpan.FromDays(7));
    }
}
