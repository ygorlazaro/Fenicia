using System.Text.Json;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.ValidateToken;

public class ValidateTokenHandler(IConnectionMultiplexer redis)
{
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();
    
    public async Task<bool> Handle(ValidateTokenQuery query, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(query.RefreshToken);

        var key = RedisPrefix + query.RefreshToken;
        var value = await this.redisDb.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            return false;
        }

        var tokenObj = JsonSerializer.Deserialize<ValidateTokenResponse>((string)value!);

        return tokenObj != null && tokenObj.UserId == query.UserId && tokenObj.IsActive
               && tokenObj.ExpirationDate > DateTime.UtcNow;
    }
}