using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs.Queries;
using Fenicia.Auth.Domains.RefreshToken.DTOs.Responses;
using Fenicia.Common.Exceptions;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class ValidateTokenServiceTests
{
    private readonly Mock<IConnectionMultiplexer> redisMock;
    private readonly Mock<IDatabase> redisDbMock;

    public ValidateTokenServiceTests()
    {
        redisMock = new Mock<IConnectionMultiplexer>();
        redisDbMock = new Mock<IDatabase>();

        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(redisDbMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsValidAndActive_ReturnsTrue()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "valid_refresh_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(5), userId, true);

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenDoesNotExistInRedis_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "non_existent_token";
        const string key = $"refresh_token:{refreshToken}";

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsInactive_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "inactive_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(5), userId, false);

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsExpired_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "expired_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(-1), userId, true);

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenBelongsToDifferentUser_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        const string refreshToken = "wrong_user_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddDays(5), differentUserId, true);

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenRefreshTokenIsNull_ThrowsArgumentException()
    {

        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId, null!);

        var service = new RefreshTokenService(redisMock.Object);
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await service.ValidateAsync(query.UserId, query.RefreshToken));
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsExpiringSoon_ReturnsTrue()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "expiring_soon_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow.AddHours(1), userId, true);

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenHasExactlyCurrentExpirationTime_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "exact_expiration_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new RefreshTokenModel(refreshToken, DateTime.UtcNow, userId, true);

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenMalformedJsonInRedis_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "malformed_token";
        const string key = $"refresh_token:{refreshToken}";

        var redisResult = new RedisValue("invalid_json");

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenRefreshTokenIsWhitespace_ThrowsArgumentException()
    {

        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId, "   ");

        var service = new RefreshTokenService(redisMock.Object);
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await service.ValidateAsync(query.UserId, query.RefreshToken));
    }

    [Fact]
    public async Task ValidateAsync_WhenRedisThrowsException_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "redis_error_token";
        const string key = $"refresh_token:{refreshToken}";

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ThrowsAsync(new RedisConnectionException(ConnectionFailureType.None, "Connection failed"));

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenIsNullAfterDeserialization_ReturnsFalse()
    {

        var userId = Guid.NewGuid();
        const string refreshToken = "null_deserialize_token";
        const string key = $"refresh_token:{refreshToken}";

        var redisResult = new RedisValue("null");

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }
}
