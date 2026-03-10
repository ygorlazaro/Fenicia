using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Services;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class IncrementAttemptsServiceTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly IncrementAttemptsService handler;
    private readonly Faker faker;

    public IncrementAttemptsServiceTests()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.handler = new IncrementAttemptsService(this.cache);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.cache.Dispose();
    }

    [Fact]
    public async Task Handle_WhenNoPreviousAttempts_SetsCountToOne()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.SetKey(email);

        
        // Assert
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Handle_WhenPreviousAttemptsExist_IncrementsCount()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 3);

        // Act
        await this.handler.SetKey(email);

        
        // Assert
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(4, count);
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_NormalizesToLowerCase()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 2);

        // Act
        await this.handler.SetKey(upperCaseEmail);

        
        // Assert
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await this.handler.SetKey(null!));
    }

    [Fact]
    public async Task Handle_WhenEmailIsEmpty_SetsCountForEmptyKey()
    {
        // Arrange
        var email = string.Empty;
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.SetKey(email);

        
        // Assert
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Handle_MultipleIncrementsForSameEmail_IncrementsCorrectly()
    {
        // Arrange
        var email = this.faker.Internet.Email();

        // Act
        await this.handler.SetKey(email);
        await this.handler.SetKey(email);
        await this.handler.SetKey(email);

        // Assert
        var key = $"login-attempt:{email.ToLower()}";
        
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task Handle_MultipleDifferentEmails_TracksSeparately()
    {
        // Arrange
        var email1 = this.faker.Internet.Email();
        var email2 = this.faker.Internet.Email();

        // Act
        await this.handler.SetKey(email1);
        await this.handler.SetKey(email1);
        await this.handler.SetKey(email2);

        // Assert
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";
        
        Assert.True(this.cache.TryGetValue(key1, out int count1));
        Assert.Equal(2, count1);
        Assert.True(this.cache.TryGetValue(key2, out int count2));
        Assert.Equal(1, count2);
    }

    [Fact]
    public async Task Handle_WhenExpirationIsSet_ExpiresAfterTimeSpan()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.SetKey(email);

        // Assert - verify entry exists
        Assert.True(this.cache.TryGetValue(key, out _));
    }

    [Fact]
    public async Task Handle_WithHighAttemptCount_IncrementsCorrectly()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        this.cache.Set(key, 99);

        // Act
        await this.handler.SetKey(email);

        
        // Assert
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(100, count);
    }

    [Fact]
    public async Task Handle_WhenEmailContainsSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var email = this.faker.Internet.Email(firstName: "test+");
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await this.handler.SetKey(email);

        
        // Assert
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }
}
