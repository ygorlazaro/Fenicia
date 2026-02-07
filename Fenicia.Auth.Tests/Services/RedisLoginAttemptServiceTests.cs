using Fenicia.Auth.Domains.LoginAttempt;

using Moq;

using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Services;

public class RedisLoginAttemptServiceTests
{
    private Mock<IDatabase> dbMock = null!;
    private Mock<IConnectionMultiplexer> muxMock = null!;
    private RedisLoginAttemptService sut = null!;

    [SetUp]
    public void Setup()
    {
        this.muxMock = new Mock<IConnectionMultiplexer>(MockBehavior.Strict);
        this.dbMock = new Mock<IDatabase>(MockBehavior.Strict);

        this.muxMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(this.dbMock.Object);

        this.sut = new RedisLoginAttemptService(this.muxMock.Object);
    }

    [Test]
    public async Task GetAttemptsAsync_ReturnsZeroWhenNoValue()
    {
        const string email = "none@ex.com";
        const string key = "login-attempt:none@ex.com";

        this.dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        var attempts = await this.sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(attempts, Is.Zero);

        this.dbMock.VerifyAll();
    }

    [Test]
    public async Task GetAttemptsAsync_ReturnsValueWhenSet()
    {
        const string email = "me@ex.com";
        const string key = "login-attempt:me@ex.com";

        this.dbMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(2);

        var attempts = await this.sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(attempts, Is.EqualTo(2));
        this.dbMock.VerifyAll();
    }

    [Test]
    public async Task IncrementAttemptsAsync_SetsExpireWhenFirstIncrement()
    {
        var email = "first@ex.com";
        var key = "login-attempt:first@ex.com";

        this.dbMock.Setup(d => d.StringIncrementAsync(key, It.IsAny<long>(), It.IsAny<CommandFlags>())).ReturnsAsync(1);
        this.dbMock.Setup(d =>
                d.KeyExpireAsync(key, It.IsAny<TimeSpan?>(), It.IsAny<ExpireWhen>(), It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await this.sut.IncrementAttemptsAsync(email);

        this.dbMock.VerifyAll();
    }

    [Test]
    public async Task ResetAttemptsAsync_DeletesKey()
    {
        var email = "del@ex.com";
        var key = "login-attempt:del@ex.com";

        this.dbMock.Setup(d => d.KeyDeleteAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(true);

        await this.sut.ResetAttemptsAsync(email, CancellationToken.None);

        this.dbMock.VerifyAll();
    }
}