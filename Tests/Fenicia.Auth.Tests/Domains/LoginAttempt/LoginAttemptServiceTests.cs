using Bogus;
using Fenicia.Auth.Domains.LoginAttempt;
using Moq;
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
        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns((RedisValue)3);

        var result = _service.GetAttempts(email);

        Assert.Equal(3, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailHasDifferentCase_ReturnsCorrectCount()
    {
        var email = _faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns((RedisValue)5);

        var result = _service.GetAttempts(email.ToUpper());

        Assert.Equal(5, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _service.GetAttempts(null!));
    }

    [Fact]
    public async Task IncrementAsync_WhenNoPreviousAttempts_SetsCountToOne()
    {
        var email = _faker.Internet.Email();

        await _service.IncrementAsync(email, CancellationToken.None);

        _redisDbMock.Verify(
            x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.Is<RedisValue>(v => (int)v == 1),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()),
            Times.Once);
    }

    [Fact]
    public async Task IncrementAsync_WhenPreviousAttemptsExist_IncrementsCount()
    {
        var email = _faker.Internet.Email();

        _redisDbMock.Setup(x => x.StringGet(It.IsAny<RedisKey>())).Returns((RedisValue)3);

        await _service.IncrementAsync(email, CancellationToken.None);

        _redisDbMock.Verify(
            x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.Is<RedisValue>(v => (int)v == 4),
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()),
            Times.Once);
    }

    [Fact]
    public async Task IncrementAsync_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.IncrementAsync(null!, CancellationToken.None));
    }

    [Fact]
    public async Task ResetAsync_WhenAttemptsExist_RemovesAttempts()
    {
        var email = _faker.Internet.Email();
        var key = $"login-attempt:{email.ToLowerInvariant()}";

        await _service.ResetAsync(email, CancellationToken.None);

        _redisDbMock.Verify(x => x.KeyDelete(It.Is<RedisKey>(k => k == key), It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task ResetAsync_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.ResetAsync(null!, CancellationToken.None));
    }

    public void Dispose()
    {
    }
}
