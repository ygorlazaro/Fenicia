using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.GenerateRefreshToken;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken.GenerateRefreshToken;

[TestFixture]
public class GenerateRefreshTokenHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        this.redisMock = new Mock<IConnectionMultiplexer>();
        this.redisDbMock = new Mock<IDatabase>();

        this.redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>()))
            .Returns(this.redisDbMock.Object);

        this.handler = new GenerateRefreshTokenHandler(this.redisMock.Object);
    }

    private Mock<IConnectionMultiplexer> redisMock = null!;
    private Mock<IDatabase> redisDbMock = null!;
    private GenerateRefreshTokenHandler handler = null!;

    [Test]
    public void Handle_GeneratesValidRefreshToken()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = this.handler.Handle(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Empty);
            Assert.That(Convert.FromBase64String(result), Has.Length.EqualTo(32), "Token should be base64 of 32 bytes");
        }
    }

    [Test]
    public void Handle_GeneratesUniqueTokensForEachCall()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var token1 = this.handler.Handle(userId);
        var token2 = this.handler.Handle(userId);
        var token3 = this.handler.Handle(userId);

        using (Assert.EnterMultipleScope())
        {
            // Assert
            Assert.That(token1, Is.Not.EqualTo(token2));
            Assert.That(token2, Is.Not.EqualTo(token3));
        }

        Assert.That(token1, Is.Not.EqualTo(token3));
    }

    [Test]
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

    [Test]
    public void Handle_SavesTokenToRedisWithCorrectValue()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = this.handler.Handle(userId);

        // Assert
        this.redisDbMock.Verify(
            x => x.StringSet(
                It.IsAny<RedisKey>(),
                It.Is<RedisValue>(v => IsValidToken(v, result, userId)),
                It.IsAny<TimeSpan>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()
            ),
            Times.Once
        );
    }

    private static bool IsValidToken(RedisValue value, string result, Guid userId)
    {
        var tokenObj = JsonSerializer.Deserialize<RefreshTokenModel>((string)value!);

        return tokenObj != null
               && tokenObj.Token == result
               && tokenObj.UserId == userId
               && tokenObj.IsActive;
    }

    [Test]
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

    [Test]
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

    [Test]
    public void Handle_ForDifferentUsers_GeneratesDifferentTokens()
    {
        // Arrange
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        // Act
        var token1 = this.handler.Handle(userId1);
        var token2 = this.handler.Handle(userId2);

        // Assert
        Assert.That(token1, Is.Not.EqualTo(token2));
    }

    [Test]
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

    [Test]
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
        Assert.That(distinctTokens, Has.Count.EqualTo(10), "All tokens should be unique");
    }
}