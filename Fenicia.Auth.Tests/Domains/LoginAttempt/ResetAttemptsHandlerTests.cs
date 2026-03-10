using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.ResetAttempts;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class ResetAttemptsHandlerTests : IDisposable
{
    public ResetAttemptsHandlerTests()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.handler = new ResetAttemptsHandler(this.cache);
        this.faker = new Faker();
    }

    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly ResetAttemptsHandler handler;

    [Fact]
    public async Task Handle_WhenAttemptsExist_RemovesAttempts()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 5);

        // Act
        await this.handler.Handle(email);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_WhenNoAttemptsExist_CompletesSuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        await this.handler.Handle(email);

        // Assert
        // Should complete successfully even when no attempts exist
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_RemovesCorrectAttempts()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 3);

        // Act
        await this.handler.Handle(upperCaseEmail);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await this.handler.Handle(null!));
    }

    [Fact]
    public async Task Handle_WhenEmailIsEmpty_RemovesEmptyKey()
    {
        // Arrange
        var email = string.Empty;
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 2);

        // Act
        await this.handler.Handle(email);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_WhenMultipleEmailsExist_RemovesOnlySpecifiedEmail()
    {
        // Arrange
        var email1 = this.faker.Internet.Email();
        var email2 = this.faker.Internet.Email();
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";
        this.cache.Set(key1, 2);
        this.cache.Set(key2, 4);

        // Act
        await this.handler.Handle(email1);

        // Assert
        Assert.False(this.cache.TryGetValue(key1, out _));
        Assert.True(this.cache.TryGetValue(key2, out int count));
        Assert.Equal(4, count);
    }

    [Fact]
    public async Task Handle_WhenHighAttemptCountExists_RemovesSuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 100);

        // Act
        await this.handler.Handle(email);

        // Assert
        var exists = this.cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_MultipleResetsForSameEmail_CompletesSuccessfully()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        await this.handler.Handle(email);
        await this.handler.Handle(email);
        await this.handler.Handle(email);

        // Assert - Should handle multiple resets without errors
    }

    public void Dispose()
    {
        this.cache.Dispose();
    }
}
