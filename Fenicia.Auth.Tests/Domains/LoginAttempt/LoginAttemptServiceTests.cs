using Bogus;
using Moq;

using Fenicia.Auth.Domains.LoginAttempt;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class LoginAttemptServiceTests : IDisposable
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _redisDbMock;
    private readonly Faker _faker;
    private readonly LoginAttemptService _service;

    public LoginAttemptServiceTests()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _redisDbMock = new Mock<IDatabase>();
        _faker = new Faker();

        _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(_redisDbMock.Object);

        _service = new LoginAttemptService(_redisMock.Object);
    }

    [Fact]
    public void GetAttempts_WhenNoAttemptsExist_ReturnsZero()
    {
        _redisDbMock.Setup(x => x.StringGet(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(RedisValue.Null);

        var result = _service.GetAttempts(_faker.Internet.Email());

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetAttempts_WhenAttemptsExist_ReturnsAttemptCount()
    {
        var email = _faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns(3);

        var result = _service.GetAttempts(email);

        Assert.Equal(3, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailHasDifferentCase_ReturnsCorrectCount()
    {
        var email = _faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns(5);

        var result = _service.GetAttempts(email.ToUpper());

        Assert.Equal(5, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _service.GetAttempts(null!));
    }
}
