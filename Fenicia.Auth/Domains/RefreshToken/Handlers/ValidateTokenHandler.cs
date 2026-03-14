using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.Queries;
using Fenicia.Auth.Domains.RefreshToken.Responses;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;

using StackExchange.Redis;

namespace Fenicia.Auth.Domains.RefreshToken.Handlers;

/// <summary>
/// Handler responsible for validating refresh tokens.
/// Checks token existence, ownership, active status, and expiration.
/// </summary>
public class ValidateTokenHandler(IConnectionMultiplexer redis)
{
    /// <summary>
    /// Redis key prefix for refresh tokens.
    /// </summary>
    private const string RedisPrefix = "refresh_token:";
    private readonly IDatabase redisDb = redis.GetDatabase();

    /// <summary>
    /// Validates a refresh token for a specific user.
    /// </summary>
    /// <param name="query">Query containing user ID and refresh token.</param>
    /// <returns>True if token is valid, active, not expired, and belongs to the user.</returns>
    /// <exception cref="InvalidRequestException">Thrown when refresh token is null or whitespace.</exception>
    /// <remarks>
    /// Validation checks:
    /// 1. Token is not null or whitespace
    /// 2. Token exists in Redis
    /// 3. Token is marked as active
    /// 4. Token has not expired (ExpirationDate > UtcNow)
    /// 5. Token belongs to the specified user
    /// Returns false gracefully for any errors (no exceptions thrown for invalid tokens).
    /// </remarks>
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
