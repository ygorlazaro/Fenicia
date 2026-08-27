using Bogus;

using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.LoginAttempt.DTOs.Commands;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class ResetAttemptsServiceTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly LoginAttemptService service;

    public ResetAttemptsServiceTests()
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
    public async Task ResetAsync_WhenAttemptsExist_RemovesAttempts()
    {
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 5);

        await service.ResetAsync(email);

        Assert.False(cache.TryGetValue(key, out _));
    }

    [Fact]
    public async Task ResetAsync_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.ResetAsync(null!));
    }
}
