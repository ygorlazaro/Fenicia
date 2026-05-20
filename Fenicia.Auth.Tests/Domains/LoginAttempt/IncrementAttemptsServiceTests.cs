using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Handlers;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class IncrementLoginAttemptsHandlerTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly IncrementLoginAttemptsHandler handler;

    public IncrementLoginAttemptsHandlerTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        handler = new IncrementLoginAttemptsHandler(cache);
        faker = new Faker();
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    [Fact]
    public async Task Handle_WhenNoPreviousAttempts_SetsCountToOne()
    {
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        await handler.IncrementAsync(email);

        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Handle_WhenPreviousAttemptsExist_IncrementsCount()
    {
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 3);

        await handler.IncrementAsync(email);

        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(4, count);
    }

    [Fact]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.IncrementAsync(null!));
    }
}
