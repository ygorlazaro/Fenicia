using System.Security.Cryptography;
using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs;
using Fenicia.Common.Exceptions;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class RefreshTokenServiceTests : IDisposable
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _redisDbMock;

    public RefreshTokenServiceTests()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _redisDbMock = new Mock<IDatabase>();

        _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(_redisDbMock.Object);
    }

    [Fact]
    public async Task GenerateAsync_GeneratesValidRefreshToken()
    {
        var userId = Guid.NewGuid();
        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));

        var result = await service.GenerateAsync(userId);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(32, Convert.FromBase64String(result).Length);
    }

    [Fact]
    public async Task GenerateAsync_GeneratesUniqueTokensForEachCall()
    {
        var userId = Guid.NewGuid();
        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));

        var token1 = await service.GenerateAsync(userId);
        var token2 = await service.GenerateAsync(userId);
        var token3 = await service.GenerateAsync(userId);

        Assert.NotEqual(token1, token2);
        Assert.NotEqual(token2, token3);
        Assert.NotEqual(token1, token3);
    }

    [Fact]
    public async Task GenerateAsync_ForDifferentUsers_GeneratesDifferentTokens()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));

        var token1 = await service.GenerateAsync(userId1);
        var token2 = await service.GenerateAsync(userId2);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public async Task GenerateAsync_MultipleTokensForSameUser_AreUnique()
    {
        var userId = Guid.NewGuid();
        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        var tokens = new List<string>();

        for (var i = 0; i < 10; i++)
        {
            tokens.Add(await service.GenerateAsync(userId));
        }

        var distinctTokens = tokens.Distinct().ToList();
        Assert.Equal(10, distinctTokens.Count);
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenDoesNotExist_ReturnsSilently()
    {
        const string refreshToken = "non_existent_token";

        _redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Never);
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsNull_ThrowsArgumentNullException()
    {
        string? refreshToken = null;

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.InvalidateAsync(refreshToken!));
    }

    [Fact]
    public async Task InvalidateAsync_WhenTokenIsEmptyString_ReturnsSilently()
    {
        var refreshToken = string.Empty;

        _redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Never);
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Once);
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Once);
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        var updatedToken = tokenResponse with { IsActive = false };
        var updatedValue = JsonSerializer.Serialize(updatedToken);
        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task InvalidateAsync_WhenMalformedJsonInRedis_ReturnsSilently()
    {
        const string refreshToken = "malformed_token";

        var redisResult = new RedisValue("invalid_json");

        _redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Never);
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

        _redisDbMock.SetupSequence(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);
        await service.InvalidateAsync(refreshToken);

        var updatedValue = JsonSerializer.Serialize(tokenResponse with { IsActive = false });
        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Once);
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    [Obsolete("Migrate the RedisConnectionException constructor overload")]
    public async Task InvalidateAsync_WhenRedisThrowsException_SilentlyIgnores()
    {
        const string refreshToken = "redis_error_token";
        const string key = $"refresh_token:{refreshToken}";

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ThrowsAsync(new RedisConnectionException(ConnectionFailureType.None, "Connection failed"));

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);
    }

    [Fact]
    public async Task InvalidateAsync_WhenJsonDeserializationFails_ReturnsSilently()
    {
        const string refreshToken = "bad_json_token";

        var redisResult = new RedisValue("not_valid_json{{{");

        _redisDbMock.Setup(x => x.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await service.InvalidateAsync(refreshToken);

        _redisDbMock.Verify(x => x.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<Expiration>(), It.IsAny<ValueCondition>(), It.IsAny<CommandFlags>()), Times.Never);
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.True(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenTokenDoesNotExistInRedis_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "non_existent_token";
        const string key = $"refresh_token:{refreshToken}";

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenRefreshTokenIsNull_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId, null!);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateAsync_WhenRefreshTokenIsWhitespace_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId, "   ");

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await service.ValidateAsync(query.UserId, query.RefreshToken));
    }

    [Fact]
    [Obsolete]
    public async Task ValidateAsync_WhenRedisThrowsException_ReturnsFalse()
    {
        var userId = Guid.NewGuid();
        const string refreshToken = "redis_error_token";
        const string key = $"refresh_token:{refreshToken}";

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ThrowsAsync(new RedisConnectionException(ConnectionFailureType.None, "Connection failed"));

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
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

        _redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>())).ReturnsAsync(redisResult);

        var service = new RefreshTokenService(new RefreshTokenRepository(_redisMock.Object));
        var result = await service.ValidateAsync(userId, refreshToken);

        Assert.False(result);
    }

    public void Dispose()
    {
    }
}
