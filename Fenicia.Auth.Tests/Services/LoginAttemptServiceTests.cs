using Fenicia.Auth.Domains.LoginAttempt;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Services;

public class LoginAttemptServiceTests
{
    private IMemoryCache cache = null!;
    private LoginAttemptService sut = null!;

    [SetUp]
    public void Setup()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.sut = new LoginAttemptService(this.cache);
    }

    [TearDown]
    public void TearDown()
    {
        this.cache.Dispose();
    }

    [Test]
    public async Task GetAttemptsAsync_ReturnsZeroWhenNotPresent()
    {
        var attempts = await this.sut.GetAttemptsAsync("noone@example.com", CancellationToken.None);

        Assert.That(attempts, Is.Zero);
    }

    [Test]
    public async Task IncrementAttemptsAsync_IncrementsAndGetReturnsCount()
    {
        var email = "user@example.com";

        await this.sut.IncrementAttemptsAsync(email);
        var first = await this.sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(first, Is.EqualTo(1));

        await this.sut.IncrementAttemptsAsync(email);
        var second = await this.sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(second, Is.EqualTo(2));
    }

    [Test]
    public async Task ResetAttemptsAsync_RemovesKey()
    {
        var email = "reset@example.com";

        await this.sut.IncrementAttemptsAsync(email);
        var before = await this.sut.GetAttemptsAsync(email, CancellationToken.None);
        Assert.That(before, Is.GreaterThan(0));

        await this.sut.ResetAttemptsAsync(email, CancellationToken.None);
        var after = await this.sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(after, Is.Zero);
    }
}