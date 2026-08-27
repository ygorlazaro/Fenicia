using Bogus;

using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.LoginAttempt.DTOs.Commands;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class LoginAttemptServiceTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly LoginAttemptService service;

    public LoginAttemptServiceTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        faker = new Faker();
        service = new LoginAttemptService(cache);
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    [Fact]
    public void GetAttempts_WhenNoAttemptsExist_ReturnsZero()
    {
        var result = service.GetAttempts(faker.Internet.Email());

        Assert.Equal(0, result);
    }

    [Fact]
    public void GetAttempts_WhenAttemptsExist_ReturnsAttemptCount()
    {
        var email = faker.Internet.Email();
        cache.Set($"login-attempt:{email.ToLower()}", 3);

        var result = service.GetAttempts(email);

        Assert.Equal(3, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailHasDifferentCase_ReturnsCorrectCount()
    {
        var email = faker.Internet.Email();
        cache.Set($"login-attempt:{email.ToLower()}", 5);

        var result = service.GetAttempts(email.ToUpper());

        Assert.Equal(5, result);
    }

    [Fact]
    public void GetAttempts_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => service.GetAttempts(null!));
    }
}
