using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.InvalidateRefreshToken;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

[TestFixture]
public class InvalidateRefreshTokenHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        this.redisMock = new Mock<IConnectionMultiplexer>();
        this.redisDbMock = new Mock<IDatabase>();

        this.redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
            .Returns(this.redisDbMock.Object);

        this.handler = new InvalidateRefreshTokenHandler(this.redisMock.Object);
    }

    private Mock<IConnectionMultiplexer> redisMock = null!;
    private Mock<IDatabase> redisDbMock = null!;
    private InvalidateRefreshTokenHandler handler = null!;

    [Test]
    public async Task Handler_WhenTokenExists_SetsIsActiveToFalse()
    {
        // Arrange
        const string refreshToken = "valid_refresh_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new InvalidateRefreshTokenResponse(
            refreshToken,
            expirationDate,
            userId
        )
        {
            IsActive = true
        };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.Is<RedisValue>(v =>
                    JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)v!)!.IsActive == false),
                It.Is<TimeSpan?>(t => t == TimeSpan.FromDays(7)),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task Handler_WhenTokenDoesNotExist_ReturnsSilently()
    {
        // Arrange
        const string refreshToken = "non_existent_token";
        const string key = $"refresh_token:{refreshToken}";

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<bool>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Never
        );
    }

    [Test]
    public void Handler_WhenTokenIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? refreshToken = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await this.handler.Handler(refreshToken!)
        );
    }

    [Test]
    public async Task Handler_WhenTokenIsEmptyString_ReturnsSilently()
    {
        // Arrange
        var refreshToken = string.Empty;
        var key = $"refresh_token:{refreshToken}";

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>()
            ),
            Times.Never
        );
    }

    [Test]
    public async Task Handler_WhenTokenIsAlreadyInactive_StillUpdates()
    {
        // Arrange
        const string refreshToken = "already_inactive_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new InvalidateRefreshTokenResponse(
            refreshToken,
            expirationDate,
            userId
        )
        {
            IsActive = false
        };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.Is<RedisValue>(v =>
                    JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)v!)!.IsActive == false),
                It.Is<TimeSpan?>(t => t == TimeSpan.FromDays(7)),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task Handler_WhenTokenIsExpired_StillInvalidates()
    {
        // Arrange
        const string refreshToken = "expired_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(-1);

        var tokenResponse = new InvalidateRefreshTokenResponse(
            refreshToken,
            expirationDate,
            userId
        )
        {
            IsActive = true
        };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.Is<RedisValue>(v =>
                    JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)v!)!.IsActive == false),
                It.Is<TimeSpan?>(t => t == TimeSpan.FromDays(7)),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task Handler_PreservesOtherTokenProperties()
    {
        // Arrange
        const string refreshToken = "token_to_invalidate";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new InvalidateRefreshTokenResponse(
            refreshToken,
            expirationDate,
            userId
        )
        {
            IsActive = true
        };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.Is<RedisValue>(v => IsValidToken(v, refreshToken, userId, expirationDate)),
                It.Is<TimeSpan?>(t => t == TimeSpan.FromDays(7)),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    private static bool IsValidToken(RedisValue v, string refreshToken, Guid userId, DateTime expirationDate)
    {
        var updatedToken = JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)v!);

        return updatedToken?.Token == refreshToken
               && updatedToken.UserId == userId
               && updatedToken.ExpirationDate == expirationDate;
    }

    [Test]
    public async Task Handler_WhenMalformedJsonInRedis_ReturnsSilently()
    {
        // Arrange
        const string refreshToken = "malformed_token";
        const string key = $"refresh_token:{refreshToken}";

        var redisResult = new RedisValue("invalid_json");

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>()
            ),
            Times.Never
        );
    }

    [Test]
    public async Task Handler_MultipleInvalidationsForSameToken_WorksCorrectly()
    {
        // Arrange
        const string refreshToken = "multi_invalidate_token";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new InvalidateRefreshTokenResponse(
            refreshToken,
            expirationDate,
            userId
        )
        {
            IsActive = true
        };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.SetupSequence(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult)
            .ReturnsAsync(RedisValue.Null);

        // Act
        await this.handler.Handler(refreshToken);
        await this.handler.Handler(refreshToken);

        // Assert - First call should update, second call should do nothing
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.IsAny<RedisValue>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    [Test]
    public async Task Handler_VerifiesCorrectTTLIsSet()
    {
        // Arrange
        const string refreshToken = "token_with_ttl";
        const string key = $"refresh_token:{refreshToken}";
        var userId = Guid.NewGuid();
        var expirationDate = DateTime.UtcNow.AddDays(5);

        var tokenResponse = new InvalidateRefreshTokenResponse(
            refreshToken,
            expirationDate,
            userId
        )
        {
            IsActive = true
        };

        var redisValue = JsonSerializer.Serialize(tokenResponse);
        var redisResult = new RedisValue(redisValue);

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        // Act
        await this.handler.Handler(refreshToken);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.Is<TimeSpan?>(t => t == TimeSpan.FromDays(7)),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }
}
