using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Handlers;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class GetLoginAttemptsHandlerTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly GetLoginAttemptsHandler handler;

    public GetLoginAttemptsHandlerTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        faker = new Faker();
        handler = new GetLoginAttemptsHandler(cache);
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    [Fact]
    public void Handle_WhenNoAttemptsExist_ReturnsZero()
    {
        var result = handler.GetAttempts(faker.Internet.Email());

        Assert.Equal(0, result);
    }

    [Fact]
    public void Handle_WhenAttemptsExist_ReturnsAttemptCount()
    {
        var email = faker.Internet.Email();
        cache.Set($"login-attempt:{email.ToLower()}", 3);

        var result = handler.GetAttempts(email);

        Assert.Equal(3, result);
    }

    [Fact]
    public void Handle_WhenEmailHasDifferentCase_ReturnsCorrectCount()
    {
        var email = faker.Internet.Email();
        cache.Set($"login-attempt:{email.ToLower()}", 5);

        var result = handler.GetAttempts(email.ToUpper());

        Assert.Equal(5, result);
    }

    [Fact]
    public void Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => handler.GetAttempts(null!));
    }
}
