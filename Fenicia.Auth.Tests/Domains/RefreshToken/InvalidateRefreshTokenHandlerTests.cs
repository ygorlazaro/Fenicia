using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs.Queries;
using Fenicia.Auth.Domains.RefreshToken.DTOs.Responses;
using Fenicia.Common.Exceptions;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class InvalidateRefreshTokenServiceTests
{
    private readonly Mock<IConnectionMultiplexer> redisMock;
    private readonly Mock<IDatabase> redisDbMock;

    public InvalidateRefreshTokenServiceTests()
    {
        redisMock = new Mock<IConnectionMultiplexer>();
        redisDbMock = new Mock<IDatabase>();

        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(redisDbMock.Object);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenExists_SetsIsActiveToFalse()
    {

        const string refreshToken = "valid_refresh_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        redisDbMock.Verify(x => x.StringSetAsync(key, updatedValue, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenDoesNotExist_ReturnsSilently()
    {

        const string refreshToken = "non_existent_token";

        redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsNull_ThrowsArgumentNullException()
    {

        string? refreshToken = null;

        var service = new RefreshTokenService(redisMock.Object);
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.InvalidateAsync(refreshToken!));
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsEmptyString_ReturnsSilently()
    {

        var refreshToken = string.Empty;

        redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsAlreadyInactive_StillUpdates()
    {

        const string refreshToken = "already_inactive_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = false };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        redisDbMock.Verify(x => x.StringSetAsync(key, updatedValue, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsExpired_StillInvalidates()
    {

        const string refreshToken = "expired_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(-1);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        redisDbMock.Verify(x => x.StringSetAsync(key, updatedValue, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_PreservesOtherTokenProperties()
    {

        const string refreshToken = "token_to_invalidate";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        var updatedToken = tokenResponse with { IsActive = false };
        var updatedValue = JsonSerializer.Serialize(updatedToken);
        redisDbMock.Verify(x => x.StringSetAsync(key, updatedValue, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenMalformedJsonInRedis_ReturnsSilently()
    {

        const string refreshToken = "malformed_token";

        var redisResult = new RedisValue("invalid_json");

        redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAsync_MultipleInvalidationsForSameToken_WorksCorrectly()
    {

        const string refreshToken = "multi_invalidate_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.SetupSequence(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        redisDbMock.Verify(x => x.StringSetAsync(key, updatedValue, It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_VerifiesCorrectTTLIsSet()
    {

        const string refreshToken = "token_with_ttl";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new RefreshTokenModel(refreshToken, expirationDate, userId) { IsActive = true };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.Is<TimeSpan?>(t => t.HasValue && t.Value == TimeSpan.FromDays(7)), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenRedisThrowsException_SilentlyIgnores()
    {

        const string refreshToken = "redis_error_token";
        const string key = $"refresh_token:{refreshToken}";

        redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ThrowsAsync(new RedisConnectionException(ConnectionFailureType.None, "Connection failed"));

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);
    }

    [Fact]
    public async Task InvalidateAsync_WhenJsonDeserializationFails_ReturnsSilently()
    {

        const string refreshToken = "bad_json_token";

        var redisResult = new RedisValue("not_valid_json{{{");

        redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(redisMock.Object);
        await service.InvalidateAsync(refreshToken);

        redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    private static bool IsValidToken(RedisValue v, string refreshToken, Guid userId, DateTime expirationDate)
    {
        var updatedToken = JsonSerializer.Deserialize<RefreshTokenModel>((string)v!);

        return updatedToken?.Token == refreshToken && updatedToken?.UserId == userId && updatedToken.ExpirationDate == expirationDate;
    }
}
