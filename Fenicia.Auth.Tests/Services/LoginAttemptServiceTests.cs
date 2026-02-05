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
        cache = new MemoryCache(new MemoryCacheOptions());
        sut = new LoginAttemptService(cache);
    }

    [TearDown]
    public void TearDown()
    {
        cache.Dispose();
    }

    [Test]
    public async Task GetAttemptsAsync_ReturnsZeroWhenNotPresent()
    {
        var attempts = await sut.GetAttemptsAsync("noone@example.com", CancellationToken.None);

        Assert.That(attempts, Is.Zero);
    }

    [Test]
    public async Task IncrementAttemptsAsync_IncrementsAndGetReturnsCount()
    {
        var email = "user@example.com";

        await sut.IncrementAttemptsAsync(email);
        var first = await sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(first, Is.EqualTo(1));

        await sut.IncrementAttemptsAsync(email);
        var second = await sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(second, Is.EqualTo(2));
    }

    [Test]
    public async Task ResetAttemptsAsync_RemovesKey()
    {
        var email = "reset@example.com";

        await sut.IncrementAttemptsAsync(email);
        var before = await sut.GetAttemptsAsync(email, CancellationToken.None);
        Assert.That(before, Is.GreaterThan(0));

        await sut.ResetAttemptsAsync(email, CancellationToken.None);
        var after = await sut.GetAttemptsAsync(email, CancellationToken.None);

        Assert.That(after, Is.Zero);
    }
}
