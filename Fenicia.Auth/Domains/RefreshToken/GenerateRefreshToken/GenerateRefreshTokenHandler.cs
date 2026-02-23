using System.Security.Cryptography;
using System.Text.Json;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.GenerateRefreshToken;

public class GenerateRefreshTokenHandler(IConnectionMultiplexer redis)
{
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();

    public string Handle(Guid userId)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var stringToken = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshTokenModel(stringToken, DateTime.UtcNow.AddDays(7), userId);

        Add(refreshToken);

        return refreshToken.Token;
    }

    private void Add(RefreshTokenModel refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var key = RedisPrefix + refreshToken.Token;
        var value = JsonSerializer.Serialize(refreshToken);

        this.redisDb.StringSet(
            key,
            value,
            TimeSpan.FromDays(7),
            When.Always,
            CommandFlags.None
        );
    }
}