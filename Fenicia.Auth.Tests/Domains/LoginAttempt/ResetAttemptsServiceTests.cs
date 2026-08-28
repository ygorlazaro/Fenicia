using Bogus;

using Fenicia.Auth.Domains.LoginAttempt;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class ResetAttemptsServiceTests : IDisposable
{
    private readonly MemoryCache _cache;
    private readonly Faker _faker;
    private readonly LoginAttemptService _service;

    public ResetAttemptsServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new LoginAttemptService(_cache);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _cache.Dispose();
    }

    [Fact]
    public async Task ResetAsync_WhenAttemptsExist_RemovesAttempts()
    {
        var email = _faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        _cache.Set(key, 5);

        await _service.ResetAsync(email);

        Assert.False(_cache.TryGetValue(key, out _));
    }

    [Fact]
    public async Task ResetAsync_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.ResetAsync(null!));
    }
}
