using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Services;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

/// <summary>
///     Unit tests for the ResetAttemptsService.
///     Tests resetting/removing login attempt counters from memory cache.
/// </summary>
/// <remarks>
///     These tests verify the core functionality of resetting login attempt counters:
///     - Removal of existing attempt counters
///     - Safe handling when no attempts exist
///     - Case-insensitive email handling
///     - Proper exception handling for null/empty input
///     - Isolation between different email addresses
///     - Handling of multiple reset operations
/// </remarks>
public class ResetAttemptsHandlerTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly Faker faker;
    private readonly ResetAttemptsService handler;

    public ResetAttemptsHandlerTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        handler = new ResetAttemptsService(cache);
        faker = new Faker();
    }

    public void Dispose()
    {
        cache.Dispose();
    }

    /// <summary>
    ///     Tests that when attempts exist, they are removed from cache.
    /// </summary>
    [Fact]
    public async Task Handle_WhenAttemptsExist_RemovesAttempts()
    {
        // Arrange
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 5);

        // Act
        await handler.Handle(email);

        // Assert
        var exists = cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    /// <summary>
    ///     Tests that when no attempts exist, the operation completes successfully without error.
    /// </summary>
    [Fact]
    public async Task Handle_WhenNoAttemptsExist_CompletesSuccessfully()
    {
        // Arrange
        var email = faker.Internet.Email();

        // Act
        await handler.Handle(email);

        // Assert
        // Should complete successfully even when no attempts exist
    }

    /// <summary>
    ///     Tests that uppercase email removes the correct lowercase cache entry.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_RemovesCorrectAttempts()
    {
        // Arrange
        var email = faker.Internet.Email();
        var upperCaseEmail = email.ToUpper();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 3);

        // Act
        await handler.Handle(upperCaseEmail);

        // Assert
        var exists = cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    /// <summary>
    ///     Tests that null email throws ArgumentNullException.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await handler.Handle(null!));
    }

    /// <summary>
    ///     Tests that empty email removes the empty key entry.
    /// </summary>
    [Fact]
    public async Task Handle_WhenEmailIsEmpty_RemovesEmptyKey()
    {
        // Arrange
        var email = string.Empty;
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 2);

        // Act
        await handler.Handle(email);

        // Assert
        var exists = cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    /// <summary>
    ///     Tests that resetting one email does not affect other emails.
    /// </summary>
    [Fact]
    public async Task Handle_WhenMultipleEmailsExist_RemovesOnlySpecifiedEmail()
    {
        // Arrange
        var email1 = faker.Internet.Email();
        var email2 = faker.Internet.Email();
        var key1 = $"login-attempt:{email1.ToLower()}";
        var key2 = $"login-attempt:{email2.ToLower()}";
        cache.Set(key1, 2);
        cache.Set(key2, 4);

        // Act
        await handler.Handle(email1);

        // Assert
        Assert.False(cache.TryGetValue(key1, out _));
        Assert.True(cache.TryGetValue(key2, out int count));
        Assert.Equal(4, count);
    }

    /// <summary>
    ///     Tests that high attempt counts are removed successfully.
    /// </summary>
    [Fact]
    public async Task Handle_WhenHighAttemptCountExists_RemovesSuccessfully()
    {
        // Arrange
        var email = faker.Internet.Email();
        var key = $"login-attempt:{email.ToLower()}";
        cache.Set(key, 100);

        // Act
        await handler.Handle(email);

        // Assert
        var exists = cache.TryGetValue(key, out _);
        Assert.False(exists);
    }

    /// <summary>
    ///     Tests that multiple resets for the same email complete without errors.
    /// </summary>
    [Fact]
    public async Task Handle_MultipleResetsForSameEmail_CompletesSuccessfully()
    {
        // Arrange
        var email = faker.Internet.Email();

        // Act
        await handler.Handle(email);
        await handler.Handle(email);
        await handler.Handle(email);

        // Assert - Should handle multiple resets without errors
    }
}
