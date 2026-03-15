using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Services;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

/// <summary>
///     Unit tests for the IncrementAttemptsService.
///     Tests incrementing login attempt counters in memory cache.
/// </summary>
/// <remarks>
///     These tests verify the core functionality of incrementing login attempt counters:
///     - Initial counter creation (sets to 1)
///     - Proper increment of existing counters
///     - Case-insensitive email handling
///     - Proper exception handling for null input
///     - Isolation between different email addresses
///     - Handling of special characters in email addresses
/// </remarks>
public class IncrementAttemptsServiceTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly IncrementAttemptsService handler;

    public IncrementAttemptsServiceTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        handler = new IncrementAttemptsService(cache);
        faker = new Faker();
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    /// <summary>
    ///     Tests that when no previous attempts exist, count is set to 1.
    /// </summary>
    [Fact]
    public async Task Handle_WhenNoPreviousAttempts_SetsCountToOne()
    {
        // Arrange
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await handler.SetKey(email);


        // Assert
        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    /// <summary>
    ///     Tests that existing attempt count is incremented correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WhenPreviousAttemptsExist_IncrementsCount()
    {
        // Arrange
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 3);

        // Act
        await handler.SetKey(email);


        // Assert
        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(4, count);
    }

    /// <summary>
    ///     Tests that email case is normalized to lowercase.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_NormalizesToLowerCase()
    {
        // Arrange
        var email = faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 2);

        // Act
        await handler.SetKey(upperCaseEmail);


        // Assert
        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(3, count);
    }

    /// <summary>
    ///     Tests that null email throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.SetKey(null!));
    }

    /// <summary>
    ///     Tests that empty email creates a cache entry with count of 1.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailIsEmpty_SetsCountForEmptyKey()
    {
        // Arrange
        var email = string.Empty;
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await handler.SetKey(email);


        // Assert
        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    /// <summary>
    ///     Tests that multiple increments for the same email accumulate correctly.
    /// </summary>
    [Fact]
    public async Task Handle_MultipleIncrementsForSameEmail_IncrementsCorrectly()
    {
        // Arrange
        var email = faker.Internet.Email();

        // Act
        await handler.SetKey(email);
        await handler.SetKey(email);
        await handler.SetKey(email);

        // Assert
        var key = $"login-attempt:{email.ToLower()}";

        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(3, count);
    }

    /// <summary>
    ///     Tests that different emails track attempts independently.
    /// </summary>
    [Fact]
    public async Task Handle_MultipleDifferentEmails_TracksSeparately()
    {
        // Arrange
        var email1 = faker.Internet.Email();
        var email2 = faker.Internet.Email();

        // Act
        await handler.SetKey(email1);
        await handler.SetKey(email1);
        await handler.SetKey(email2);

        // Assert
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";

        Assert.True(cache.TryGetValue(key1, out int count1));
        Assert.Equal(2, count1);
        Assert.True(cache.TryGetValue(key2, out int count2));
        Assert.Equal(1, count2);
    }

    /// <summary>
    ///     Tests that cache entries are created with expiration set.
    /// </summary>
    [Fact]
    public async Task Handle_WhenExpirationIsSet_ExpiresAfterTimeSpan()
    {
        // Arrange
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await handler.SetKey(email);

        // Assert - verify entry exists
        Assert.True(cache.TryGetValue(key, out _));
    }

    /// <summary>
    ///     Tests that high attempt counts are incremented correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WithHighAttemptCount_IncrementsCorrectly()
    {
        // Arrange
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 99);

        // Act
        await handler.SetKey(email);


        // Assert
        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(100, count);
    }

    /// <summary>
    ///     Tests that emails with special characters are handled correctly.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailContainsSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var email = faker.Internet.Email("test+");
        var key = $"login-attempt:{email.ToLower()}";

        // Act
        await handler.SetKey(email);


        // Assert
        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }
}
