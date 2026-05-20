using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Handlers;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class ResetLoginAttemptsHandlerTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly ResetLoginAttemptsHandler handler;

    public ResetLoginAttemptsHandlerTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        handler = new ResetLoginAttemptsHandler(cache);
        faker = new Faker();
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    [Fact]
    public async Task Handle_WhenAttemptsExist_RemovesAttempts()
    {
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 5);

        await handler.ResetAsync(email);

        Assert.False(cache.TryGetValue(key, out _));
    }

    [Fact]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.ResetAsync(null!));
    }
}
