using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.Handlers;
using Fenicia.Auth.Domains.RefreshToken.Queries;
using Fenicia.Auth.Domains.RefreshToken.Responses;
using Fenicia.Common.Exceptions;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

/// <summary>
/// Unit tests for the ValidateTokenHandler.
/// Tests validation of refresh tokens stored in Redis.
/// </summary>
/// <remarks>
/// These tests verify:
/// - Valid tokens return true
/// - Non-existent tokens return false
/// - Inactive tokens return false
/// - Expired tokens return false
/// - Tokens for wrong user return false
/// - Null/whitespace tokens throw exception
/// - Edge cases (expiring soon, exact expiration, malformed JSON)
/// </remarks>
public class ValidateTokenHandlerTests
{
    private readonly Mock<IDatabase> redisDbMock;
    private readonly ValidateTokenHandler handler;

    public ValidateTokenHandlerTests()
    {
        var redisMock = new Mock<IConnectionMultiplexer>();
        this.redisDbMock = new Mock<IDatabase>();

        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(),
                It.IsAny<object?>()))
            .Returns(this.redisDbMock.Object);

        this.handler = new ValidateTokenHandler(redisMock.Object);
    }

    /// <summary>
    /// Tests that a valid, active, non-expired token belonging to the user returns true.
    /// </summary>
    [Fact]
    public async Task Handle_WhenTokenIsValidAndActive_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "valid_refresh_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(5),
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that a token that doesn't exist in Redis returns false.
    /// </summary>
    [Fact]
    public async Task Handle_WhenTokenDoesNotExistInRedis_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "non_existent_token";
        const string key = $"refresh_token:{refreshToken}";

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that an inactive token returns false.
    /// </summary>
    [Fact]
    public async Task Handle_WhenTokenIsInactive_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "inactive_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(5),
            userId,
            false
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that an expired token returns false.
    /// </summary>
    [Fact]
    public async Task Handle_WhenTokenIsExpired_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "expired_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(-1),
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that a token belonging to a different user returns false.
    /// </summary>
    [Fact]
    public async Task Handle_WhenTokenBelongsToDifferentUser_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        const string refreshToken = "wrong_user_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(5),
            differentUserId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenIsNull_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId,
            null!);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await this.handler.Handle(query));
    }

    /// <summary>
    /// Tests that a token expiring soon (within 1 hour) still returns true.
    /// </summary>
    [Fact]
    public async Task Handle_WhenTokenIsExpiringSoon_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "expiring_soon_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests that a token with expiration exactly equal to now returns false (expired).
    /// </summary>
    [Fact]
    public async Task Handle_WhenTokenHasExactlyCurrentExpirationTime_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "exact_expiration_token";
        const string key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow,
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that malformed JSON in Redis returns false gracefully.
    /// </summary>
    [Fact]
    public async Task Handle_WhenMalformedJsonInRedis_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "malformed_token";
        const string key = $"refresh_token:{refreshToken}";

        var redisResult = new RedisValue("invalid_json");

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenIsWhitespace_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId,
            "   ");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await this.handler.Handle(query));
    }

    /// <summary>
    /// Tests that when Redis throws an exception, returns false gracefully.
    /// </summary>
    [Fact]
    public async Task Handle_WhenRedisThrowsException_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "redis_error_token";
        const string key = $"refresh_token:{refreshToken}";

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ThrowsAsync(new RedisConnectionException(ConnectionFailureType.None,
                "Connection failed"));

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task Handle_WhenTokenIsNullAfterDeserialization_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        const string refreshToken = "null_deserialize_token";
        const string key = $"refresh_token:{refreshToken}";

        var redisResult = new RedisValue("null");

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId,
            refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.False(result);
    }
}
