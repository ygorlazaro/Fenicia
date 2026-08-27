using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.Commands;
using Fenicia.Auth.Domains.RefreshToken.Responses;

using MediatR;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.Handlers;

public class InvalidateRefreshTokenHandler(IConnectionMultiplexer redis) : IRequestHandler<InvalidateRefreshTokenCommand>
{
    private const string RedisPrefix = "refresh_token:";

    private readonly IDatabase redisDb = redis.GetDatabase();

    public async Task InvalidateAsync(string refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
        {
            var key = RedisPrefix + refreshToken;
            var value = await redisDb.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                return;
            }

            var tokenObj = JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)value!);

            tokenObj?.IsActive = false;

            await redisDb.StringSetAsync(key, JsonSerializer.Serialize(tokenObj), TimeSpan.FromDays(7), When.Always, CommandFlags.None);
        }
        catch
        {

        }
    }

    public Task Handle(InvalidateRefreshTokenCommand request, CancellationToken ct)
    {
        return InvalidateAsync(request.RefreshToken);
    }

    public Task Handler(string refreshToken)
    {
        return InvalidateAsync(refreshToken);
    }
}
