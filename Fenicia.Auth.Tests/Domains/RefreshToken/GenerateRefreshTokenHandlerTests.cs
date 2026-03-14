using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.Handlers;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

/// <summary>
/// Unit tests for the GenerateRefreshTokenHandler.
/// Tests generation and storage of refresh tokens in Redis.
/// </summary>
/// <remarks>
/// These tests verify:
/// - Token generation produces valid Base64 strings
/// - Tokens are unique for each call
/// - Tokens are saved to Redis with correct key format
/// - Tokens have correct expiration (7 days)
/// - Tokens are marked as active by default
/// </remarks>
public class GenerateRefreshTokenHandlerTests
{
    private readonly Mock<IDatabase> redisDbMock;
    private readonly GenerateRefreshTokenHandler handler;

    public GenerateRefreshTokenHandlerTests()
    {
        var redisMock = new Mock<IConnectionMultiplexer>();
        this.redisDbMock = new Mock<IDatabase>();

        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(),
                It.IsAny<object?>()))
            .Returns(this.redisDbMock.Object);

        this.handler = new GenerateRefreshTokenHandler(redisMock.Object);
    }

    /// <summary>
    /// Tests that generated token is a valid non-empty Base64 string of correct length.
    /// </summary>
    [Fact]
    public void Handle_GeneratesValidRefreshToken()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = this.handler.Handle(userId);

        // Assert
        Assert.NotNull(result);
        
        Assert.NotEmpty(result);
        Assert.Equal(32,
            Convert.FromBase64String(result)
                .Length);
    }

    /// <summary>
    /// Tests that multiple calls generate unique tokens.
    /// </summary>
    [Fact]
    public void Handle_GeneratesUniqueTokensForEachCall()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var token1 = this.handler.Handle(userId);
        var token2 = this.handler.Handle(userId);
        var token3 = this.handler.Handle(userId);

        
        Assert.NotEqual(token1,
            token2);
        Assert.NotEqual(token2,
            token3);

        Assert.NotEqual(token1,
            token3);
    }

    /// <summary>
    /// Tests that token is saved to Redis with correct key format.
    /// </summary>
    [Fact]
    public void Handle_SavesTokenToRedisWithCorrectKey()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = this.handler.Handle(userId);

        // Assert
        var key = $"refresh_token:{result}";
        this.redisDbMock.Verify(
            x => x.StringSet(
                It.Is<RedisKey>(k => k == key),
                It.IsAny<RedisValue>(),
                It.Is<TimeSpan>(t => t == TimeSpan.FromDays(7)),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
        Times.Once
        );
    }

    /// <summary>
    /// Tests that token expiration is set to 7 days.
    /// </summary>
    [Fact]
    public void Handle_SetsCorrectExpirationTime()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        this.handler.Handle(userId);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSet(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.Is<TimeSpan>(t => t == TimeSpan.FromDays(7)),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    /// <summary>
    /// Tests that token is created with IsActive set to true.
    /// </summary>
    [Fact]
    public void Handle_TokenIsActiveByDefault()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        this.handler.Handle(userId);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSet(
                It.IsAny<RedisKey>(),
                It.Is<RedisValue>(v => JsonSerializer.Deserialize<RefreshTokenModel>((string)v!)!.IsActive == true),
                It.IsAny<TimeSpan>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    /// <summary>
    /// Tests that different users get different tokens.
    /// </summary>
    [Fact]
    public void Handle_ForDifferentUsers_GeneratesDifferentTokens()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var token1 = this.handler.Handle(userId1);
        var token2 = this.handler.Handle(userId2);

        // Assert
        Assert.NotEqual(token1,
            token2);
    }

    /// <summary>
    /// Tests that saved token can be deserialized back to a valid object.
    /// </summary>
    [Fact]
    public void Handle_VerifiesRedisSetValueCanBeDeserialized()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        this.handler.Handle(userId);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSet(
                It.IsAny<RedisKey>(),
                It.Is<RedisValue>(v => JsonSerializer.Deserialize<RefreshTokenModel>((string)v!) != null),
                It.IsAny<TimeSpan>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    /// <summary>
    /// Tests that multiple tokens for the same user are all unique.
    /// </summary>
    [Fact]
    public void Handle_MultipleTokensForSameUser_AreUnique()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tokens = new List<string>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            tokens.Add(this.handler.Handle(userId));
        }

        // Assert
        var distinctTokens = tokens.Distinct().ToList();
        Assert.Equal(10,
            distinctTokens.Count);
    }
}
