using System.Text.Json;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.Handlers;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class GenerateRefreshTokenHandlerTests
{
    private readonly GenerateRefreshTokenHandler handler;
    private readonly Mock<IDatabase> redisDbMock;

    public GenerateRefreshTokenHandlerTests()
    {
        var redisMock = new Mock<IConnectionMultiplexer>();
        redisDbMock = new Mock<IDatabase>();

        redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(redisDbMock.Object);

        handler = new GenerateRefreshTokenHandler(redisMock.Object);
    }

    [Fact]
    public void Handle_GeneratesValidRefreshToken()
    {

        var userId = Guid.NewGuid();

        var result = handler.Handle(userId);

        Assert.NotNull(result);

        Assert.NotEmpty(result);
        Assert.Equal(32, Convert.FromBase64String(result).Length);
    }

    [Fact]
    public void Handle_GeneratesUniqueTokensForEachCall()
    {

        var userId = Guid.NewGuid();

        var token1 = handler.Handle(userId);
        var token2 = handler.Handle(userId);
        var token3 = handler.Handle(userId);

        Assert.NotEqual(token1, token2);
        Assert.NotEqual(token2, token3);

        Assert.NotEqual(token1, token3);
    }

    [Fact]
    public void Handle_SavesTokenToRedisWithCorrectKey()
    {

        var userId = Guid.NewGuid();

        var result = handler.Handle(userId);

        var key = $"refresh_token:{result}";
        redisDbMock.Verify(x => x.StringSet(It.Is<RedisKey>(k => k == key), It.IsAny<RedisValue>(), It.Is<TimeSpan>(t => t == TimeSpan.FromDays(7)), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public void Handle_SetsCorrectExpirationTime()
    {

        var userId = Guid.NewGuid();

        handler.Handle(userId);

        redisDbMock.Verify(x => x.StringSet(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.Is<TimeSpan>(t => t == TimeSpan.FromDays(7)), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public void Handle_TokenIsActiveByDefault()
    {

        var userId = Guid.NewGuid();

        handler.Handle(userId);

        redisDbMock.Verify(x => x.StringSet(It.IsAny<RedisKey>(), It.Is<RedisValue>(v => JsonSerializer.Deserialize<RefreshTokenModel>((string)v!)!.IsActive == true), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public void Handle_ForDifferentUsers_GeneratesDifferentTokens()
    {

        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();

        var token1 = handler.Handle(userId1);
        var token2 = handler.Handle(userId2);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void Handle_VerifiesRedisSetValueCanBeDeserialized()
    {

        var userId = Guid.NewGuid();

        handler.Handle(userId);

        redisDbMock.Verify(x => x.StringSet(It.IsAny<RedisKey>(), It.Is<RedisValue>(v => JsonSerializer.Deserialize<RefreshTokenModel>((string)v!) != null), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public void Handle_MultipleTokensForSameUser_AreUnique()
    {

        var userId = Guid.NewGuid();
        var tokens = new List<string>();

        for (var i = 0; i < 10; i++)
        {
            tokens.Add(handler.Handle(userId));
        }

        var distinctTokens = tokens.Distinct().ToList();
        Assert.Equal(10, distinctTokens.Count);
    }
}
