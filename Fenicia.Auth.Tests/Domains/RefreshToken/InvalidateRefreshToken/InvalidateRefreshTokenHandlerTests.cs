using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken.InvalidateRefreshToken;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken.InvalidateRefreshToken;

[TestFixture]
public class InvalidateRefreshTokenHandlerTests
{
    private Mock<IConnectionMultiplexer> redisMock = null!;
    private Mock<IDatabase> redisDbMock = null!;
    private InvalidateRefreshTokenHandler handler = null!;

    [SetUp]
    public void SetUp()
    {
        this.redisMock = new Mock<IConnectionMultiplexer>();
        this.redisDbMock = new Mock<IDatabase>();

        this.redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
            .Returns(this.redisDbMock.Object);

        this.handler = new InvalidateRefreshTokenHandler(this.redisMock.Object);
    }

    [Test]
    public async Task Handler_WhenTokenExists_SetsIsActiveToFalse()
    {
        // Arrange
        var refreshToken = "valid_refresh_token";
        var key = $"refresh_token:{refreshToken}";
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
        await this.handler.Handler(refreshToken, CancellationToken.None);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.Is<RedisValue>(v => JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)v!)!.IsActive == false),
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
        var refreshToken = "non_existent_token";
        var key = $"refresh_token:{refreshToken}";

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        // Act
        await this.handler.Handler(refreshToken, CancellationToken.None);

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
    public async Task Handler_WhenTokenIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        string? refreshToken = null;

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(
            async () => await this.handler.Handler(refreshToken!, CancellationToken.None)
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
        await this.handler.Handler(refreshToken, CancellationToken.None);

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
        var refreshToken = "already_inactive_token";
        var key = $"refresh_token:{refreshToken}";
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
        await this.handler.Handler(refreshToken, CancellationToken.None);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.Is<RedisValue>(v => JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)v!)!.IsActive == false),
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
        var refreshToken = "expired_token";
        var key = $"refresh_token:{refreshToken}";
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
        await this.handler.Handler(refreshToken, CancellationToken.None);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSetAsync(
                It.Is<RedisKey>(k => k == key),
                It.Is<RedisValue>(v => JsonSerializer.Deserialize<InvalidateRefreshTokenResponse>((string)v!)!.IsActive == false),
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
        var refreshToken = "token_to_invalidate";
        var key = $"refresh_token:{refreshToken}";
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
        await this.handler.Handler(refreshToken, CancellationToken.None);

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

    private static bool IsValidToken(RedisValue v, string refreshToken, Guid userId, DateTime  expirationDate)
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
        var refreshToken = "malformed_token";
        var key = $"refresh_token:{refreshToken}";

        var redisResult = new RedisValue("invalid_json");

        this.redisDbMock.Setup(x => x.StringGetAsync(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()))
            .ReturnsAsync(redisResult);

        // Act
        await this.handler.Handler(refreshToken, CancellationToken.None);

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
        var refreshToken = "multi_invalidate_token";
        var key = $"refresh_token:{refreshToken}";
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
        await this.handler.Handler(refreshToken, CancellationToken.None);
        await this.handler.Handler(refreshToken, CancellationToken.None);

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
        var refreshToken = "token_with_ttl";
        var key = $"refresh_token:{refreshToken}";
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
        await this.handler.Handler(refreshToken, CancellationToken.None);

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
