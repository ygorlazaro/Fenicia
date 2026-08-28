using Bogus;

using Fenicia.Auth.Domains.LoginAttempt;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class IncrementAttemptsServiceTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly LoginAttemptService service;

    public IncrementAttemptsServiceTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        service = new LoginAttemptService(cache);
        faker = new Faker();
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    [Fact]
    public async Task IncrementAsync_WhenNoPreviousAttempts_SetsCountToOne()
    {
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        await service.IncrementAsync(email);

        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task IncrementAsync_WhenPreviousAttemptsExist_IncrementsCount()
    {
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 3);

        await service.IncrementAsync(email);

        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(4, count);
    }

    [Fact]
    public async Task IncrementAsync_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.IncrementAsync(null!));
    }
}
