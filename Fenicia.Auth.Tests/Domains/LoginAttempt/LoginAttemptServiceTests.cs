using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Services;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class LoginAttemptServiceTests : IDisposable
{
    public LoginAttemptServiceTests()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.faker = new Faker();
        this._service = new LoginAttemptService(this.cache);
    }

    private readonly MemoryCache cache;
    private readonly LoginAttemptService _service;
    private readonly Faker faker;

    [Fact]
    public void Handle_WhenNoAttemptsExist_ReturnsZero()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        var result = this._service.Handle(email);

        // Assert
        Assert.Equal(0,
            result);
    }

    [Fact]
    public void Handle_WhenAttemptsExist_ReturnsAttemptCount()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key,
            3);

        // Act
        var result = this._service.Handle(email);

        // Assert
        Assert.Equal(3,
            result);
    }

    [Fact]
    public void Handle_WhenEmailHasDifferentCase_ReturnsCorrectCount()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key,
            5);

        // Act
        var result = this._service.Handle(upperCaseEmail);

        // Assert
        Assert.Equal(5,
            result);
    }

    [Fact]
    public void Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => this._service.Handle(null!));
    }

    [Fact]
    public void Handle_WhenEmailIsEmpty_ReturnsZero()
    {
        // Arrange
        var email = string.Empty;

        // Act
        var result = this._service.Handle(email);

        // Assert
        Assert.Equal(0,
            result);
    }

    [Fact]
    public void Handle_WhenEmailContainsSpaces_ReturnsZero()
    {
        // Arrange
        var email = " test@example.com ";

        // Act
        var result = this._service.Handle(email);

        // Assert
        Assert.Equal(0,
            result);
    }

    [Fact]
    public void Handle_WithMultipleDifferentEmails_ReturnsCorrectCounts()
    {
        // Arrange
        var email1 = this.faker.Internet.Email();
        var email2 = this.faker.Internet.Email();
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";
        this.cache.Set(key1,
            2);
        this.cache.Set(key2,
            4);

        // Act
        var result1 = this._service.Handle(email1);
        var result2 = this._service.Handle(email2);

        // Assert
        Assert.Equal(2,
            result1);
        Assert.Equal(4,
            result2);
    }

    [Fact]
    public void Handle_WhenAttemptCountIsHigh_ReturnsCorrectCount()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key,
            100);

        // Act
        var result = this._service.Handle(email);

        // Assert
        Assert.Equal(100,
            result);
    }

    public void Dispose()
    {
        this.cache.Dispose();
    }
}
