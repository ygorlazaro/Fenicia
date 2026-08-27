using System.Security.Cryptography;
using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.Commands;

using MediatR;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.Handlers;

public class GenerateRefreshTokenHandler(IConnectionMultiplexer redis) : IRequestHandler<GenerateRefreshTokenCommand, string>
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

    public Task<string> Handle(GenerateRefreshTokenCommand request, CancellationToken ct)
    {
        return Task.FromResult(Handle(request.UserId));
    }

    private void Add(RefreshTokenModel refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        var key = RedisPrefix + refreshToken.Token;
        var value = JsonSerializer.Serialize(refreshToken);

        redisDb.StringSet(key, value, TimeSpan.FromDays(7), When.Always, CommandFlags.None);
    }
}
