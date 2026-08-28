using Fenicia.Auth.Domains.RefreshToken;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.RefreshToken;

public class GenerateRefreshTokenServiceTests
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _redisDbMock;

    public GenerateRefreshTokenServiceTests()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _redisDbMock = new Mock<IDatabase>();

        _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(_redisDbMock.Object);
    }

    [Fact]
    public void Generate_GeneratesValidRefreshToken()
    {
        var userId = Guid.NewGuid();
        var service = new RefreshTokenService(_redisMock.Object);

        var result = service.Generate(userId);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(32, Convert.FromBase64String(result).Length);
    }

    [Fact]
    public void Generate_GeneratesUniqueTokensForEachCall()
    {
        var userId = Guid.NewGuid();
        var service = new RefreshTokenService(_redisMock.Object);

        var token1 = service.Generate(userId);
        var token2 = service.Generate(userId);
        var token3 = service.Generate(userId);

        Assert.NotEqual(token1, token2);
        Assert.NotEqual(token2, token3);
        Assert.NotEqual(token1, token3);
    }

    [Fact]
    public void Generate_ForDifferentUsers_GeneratesDifferentTokens()
    {
        var userId1 = Guid.NewGuid();
        var userId2 = Guid.NewGuid();
        var service = new RefreshTokenService(_redisMock.Object);

        var token1 = service.Generate(userId1);
        var token2 = service.Generate(userId2);

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void Generate_MultipleTokensForSameUser_AreUnique()
    {
        var userId = Guid.NewGuid();
        var service = new RefreshTokenService(_redisMock.Object);
        var tokens = new List<string>();

        for (var i = 0; i < 10; i++)
        {
            tokens.Add(service.Generate(userId));
        }

        var distinctTokens = tokens.Distinct().ToList();
        Assert.Equal(10, distinctTokens.Count);
    }
}
