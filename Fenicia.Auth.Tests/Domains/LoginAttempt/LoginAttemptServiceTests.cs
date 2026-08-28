using Bogus;

using Fenicia.Auth.Domains.LoginAttempt;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class LoginAttemptServiceTests : IDisposable
{
    private readonly MemoryCache _cache;
    private readonly Faker _faker;
    private readonly LoginAttemptService _service;

    public LoginAttemptServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _faker = new Faker();
        _service = new LoginAttemptService(_cache);
    }

    public void Dispose()
    {
        _cache.Dispose();
    }

    [Fact]
    public void GetAttempts_WhenNoAttemptsExist_ReturnsZero()
    {
        var result = _service.GetAttempts(_faker.Internet.Email());

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetAttempts_WhenAttemptsExist_ReturnsAttemptCount()
    {
        var email = _faker.Internet.Email();
        _cache.Set($"login-attempt:{email.ToLower()}", 3);

        var result = _service.GetAttempts(email);

        Assert.Equal(3, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailHasDifferentCase_ReturnsCorrectCount()
    {
        var email = _faker.Internet.Email();
        _cache.Set($"login-attempt:{email.ToLower()}", 5);

        var result = _service.GetAttempts(email.ToUpper());

        Assert.Equal(5, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _service.GetAttempts(null!));
    }
}
