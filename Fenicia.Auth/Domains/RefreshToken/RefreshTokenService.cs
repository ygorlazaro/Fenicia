using System.Security.Cryptography;
using System.Text.Json;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshTokenService(IConnectionMultiplexer redis)
{
    private const string RedisPrefix = "refresh_token:";

    private readonly IDatabase redisDb = redis.GetDatabase();

    public virtual string Generate(Guid userId)
    {
        var randomNumber = new byte[32];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var stringToken = Convert.ToBase64String(randomNumber);
        var refreshToken = new RefreshTokenModel(stringToken, DateTime.UtcNow.AddDays(7), userId);

        Add(refreshToken);

        return refreshToken.Token;
    }

    public virtual async Task InvalidateAsync(string refreshToken, CancellationToken ct = default)
    {
        if (refreshToken is null)
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        try
        {
            var key = RedisPrefix + refreshToken;
            var value = await redisDb.StringGetAsync(key, CommandFlags.None);

            if (value.IsNullOrEmpty)
            {
                return;
            }

            var token = JsonSerializer.Deserialize<RefreshTokenModel>(value.ToString());

            if (token is null)
            {
                return;
            }

            var updatedToken = token with { IsActive = false };

            await redisDb.StringSetAsync(key, JsonSerializer.Serialize(updatedToken), TimeSpan.FromDays(7), When.Always, CommandFlags.None);
        }
        catch
        {
        }
    }

    public virtual async Task<bool> ValidateAsync(Guid userId, string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        try
        {
            var key = RedisPrefix + refreshToken;
            var value = await redisDb.StringGetAsync(key, CommandFlags.None);

            if (value.IsNullOrEmpty)
            {
                return false;
            }

            var token = JsonSerializer.Deserialize<RefreshTokenModel>(value.ToString());

            return token != null && token.UserId == userId && token.IsActive && token.ExpirationDate > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }

    private void Add(RefreshTokenModel refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var key = RedisPrefix + refreshToken.Token;
        var value = JsonSerializer.Serialize(refreshToken);

        redisDb.StringSet(key, value, refreshToken.ExpirationDate, When.Always, CommandFlags.None);
    }
}
