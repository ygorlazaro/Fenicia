using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.Queries;
using Fenicia.Auth.Domains.RefreshToken.Responses;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.Handlers;

public class ValidateTokenHandler(IConnectionMultiplexer redis)
{
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();

    public async Task<bool> Handle(ValidateTokenQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.RefreshToken))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRefreshToken);
        }

        try
        {
            var key = RedisPrefix + query.RefreshToken;
            var value = await this.redisDb.StringGetAsync(key);

            if (value.IsNullOrEmpty)
            {
                return false;
            }

            var tokenObj = JsonSerializer.Deserialize<ValidateTokenResponse>((string)value!);

            return tokenObj != null && tokenObj.UserId == query.UserId && tokenObj.IsActive && tokenObj.ExpirationDate > DateTime.UtcNow;
        }
        catch
        {
            return false;
        }
    }
}
