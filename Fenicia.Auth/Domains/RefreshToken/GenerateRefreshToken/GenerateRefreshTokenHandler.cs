using System.Security.Cryptography;
using System.Text.Json;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.GenerateRefreshToken;

public class GenerateRefreshTokenHandler(IConnectionMultiplexer redis)
{
    public string Handle(Guid userId)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var refreshToken = new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = userId,
            ExpirationDate = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };
        
        Add(refreshToken);

        return refreshToken.Token;
    }
    
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();

    private void Add(RefreshToken refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var key = RedisPrefix + refreshToken.Token;
        var value = JsonSerializer.Serialize(refreshToken);

        this.redisDb.StringSet(key, value, TimeSpan.FromDays(7));
    }

}