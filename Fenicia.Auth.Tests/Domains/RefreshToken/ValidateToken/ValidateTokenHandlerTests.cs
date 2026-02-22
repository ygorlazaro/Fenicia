using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.ValidateToken;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken.ValidateToken;

[TestFixture]
public class ValidateTokenHandlerTests
{
    private Mock<IConnectionMultiplexer> redisMock = null!;
    private Mock<IDatabase> redisDbMock = null!;
    private ValidateTokenHandler handler = null!;

    [SetUp]
    public void SetUp()
    {
        this.redisMock = new Mock<IConnectionMultiplexer>();
        this.redisDbMock = new Mock<IDatabase>();

        this.redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
            .Returns(this.redisDbMock.Object);

        this.handler = new ValidateTokenHandler(this.redisMock.Object);
    }

    [Test]
    public async Task Handle_WhenTokenIsValidAndActive_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "valid_refresh_token";
        var key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(5),
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.True, "Should return true for valid and active token");
    }

    [Test]
    public async Task Handle_WhenTokenDoesNotExistInRedis_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "non_existent_token";
        var key = $"refresh_token:{refreshToken}";

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.False, "Should return false when token doesn't exist");
    }

    [Test]
    public async Task Handle_WhenTokenIsInactive_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "inactive_token";
        var key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(5),
            userId,
            false
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.False, "Should return false for inactive token");
    }

    [Test]
    public async Task Handle_WhenTokenIsExpired_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "expired_token";
        var key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(-1),
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.False, "Should return false for expired token");
    }

    [Test]
    public async Task Handle_WhenTokenBelongsToDifferentUser_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var refreshToken = "wrong_user_token";
        var key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddDays(5),
            differentUserId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.False, "Should return false when token belongs to different user");
    }

    [Test]
    public async Task Handle_WhenRefreshTokenIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var query = new ValidateTokenQuery(userId, null!);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await this.handler.Handle(query)
        );
    }

    [Test]
    public async Task Handle_WhenTokenIsExpiringSoon_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "expiring_soon_token";
        var key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.True, "Should return true for token expiring soon but still valid");
    }

    [Test]
    public async Task Handle_WhenTokenHasExactlyCurrentExpirationTime_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "exact_expiration_token";
        var key = $"refresh_token:{refreshToken}";

        var tokenResponse = new ValidateTokenResponse(
            refreshToken,
            DateTime.UtcNow,
            userId,
            true
        );

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.False, "Should return false when expiration equals current time");
    }

    [Test]
    public async Task Handle_WhenMalformedJsonInRedis_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var refreshToken = "malformed_token";
        var key = $"refresh_token:{refreshToken}";

        var redisResult = new RedisValue("invalid_json");

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        var query = new ValidateTokenQuery(userId, refreshToken);

        // Act
        var result = await this.handler.Handle(query);

        // Assert
        Assert.That(result, Is.False, "Should return false for malformed JSON in Redis");
    }
}
