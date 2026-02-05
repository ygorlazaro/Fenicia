using Fenicia.Auth.Domains.LoginAttempt;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Services;

public class RedisLoginAttemptServiceTests
{
    private Mock<IConnectionMultiplexer> muxMock = null!;
    private Mock<IDatabase> dbMock = null!;
    private RedisLoginAttemptService sut = null!;

    [SetUp]
    public void Setup()
    {
        muxMock = new Mock<IConnectionMultiplexer>(MockBehavior.Strict);
        dbMock = new Mock<IDatabase>(MockBehavior.Strict);

        muxMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(dbMock.Object);

        sut = new RedisLoginAttemptService(muxMock.Object);
    }

    [Test]
    public async Task GetAttemptsAsync_ReturnsZeroWhenNoValue()
    {
        const string email = "none@ex.com";
        const string key = "login-attempt:none@ex.com";

        dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var attempts = await sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(attempts, Is.Zero);

        dbMock.VerifyAll();
    }

    [Test]
    public async Task GetAttemptsAsync_ReturnsValueWhenSet()
    {
        const string email = "me@ex.com";
        const string key = "login-attempt:me@ex.com";

        dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(2);

        var attempts = await sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(attempts, Is.EqualTo(2));
        dbMock.VerifyAll();
    }

    [Test]
    public async Task IncrementAttemptsAsync_SetsExpireWhenFirstIncrement()
    {
        var email = "first@ex.com";
        var key = "login-attempt:first@ex.com";

        dbMock.Setup(d => d.StringIncrementAsync(key, It.IsAny<long>(), It.IsAny<CommandFlags>())).ReturnsAsync(1);
        dbMock.Setup(d => d.KeyExpireAsync(key, It.IsAny<TimeSpan?>(), It.IsAny<ExpireWhen>(), It.IsAny<CommandFlags>())).ReturnsAsync(true);

        await sut.IncrementAttemptsAsync(email);

        dbMock.VerifyAll();
    }

    [Test]
    public async Task ResetAttemptsAsync_DeletesKey()
    {
        var email = "del@ex.com";
        var key = "login-attempt:del@ex.com";

        dbMock.Setup(d => d.KeyDeleteAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(true);

        await sut.ResetAttemptsAsync(email, CancellationToken.None);

        dbMock.VerifyAll();
    }
}
