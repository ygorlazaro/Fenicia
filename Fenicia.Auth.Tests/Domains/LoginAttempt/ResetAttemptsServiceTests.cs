using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Services;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.LoginAttempt;

public class ResetAttemptsServiceTests : IDisposable
{
    // Constants for consistent test data
    private const string ValidEmail = "test.user@example.com";
    private const string ValidEmailUpperCase = "TEST.USER@EXAMPLE.COM";
    private const string ValidEmailWithWhitespace = "  test.user@example.com  ";
    private const string EmptyEmail = "";
    private const string WhitespaceEmail = "   ";

    // System Under Test (SUT) and dependencies
    private readonly MemoryCache _cache;
    private readonly Faker _faker;
    private readonly ResetAttemptsService _service;

    public ResetAttemptsServiceTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new ResetAttemptsService(_cache);
        _faker = new Faker();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _cache.Dispose();
    }

    [Fact]
    public async Task Handle_WhenAttemptsExistForValidEmail_RemovesAttemptsFromCache()
    {
        // Arrange
        var email = ValidEmail;
        var cacheKey = GetCacheKey(email);
        _cache.Set(cacheKey, 5);

        // Act
        await _service.Handle(email);

        // Assert
        var exists = _cache.TryGetValue(cacheKey, out _);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_WhenNoAttemptsExistForValidEmail_CompletesSuccessfully()
    {
        // Arrange
        var email = ValidEmail;
        var cacheKey = GetCacheKey(email);
        Assert.False(_cache.TryGetValue(cacheKey, out _));

        // Act & Assert
        await _service.Handle(email); // Should not throw any exception
    }

    [Fact]
    public async Task Handle_WhenEmailHasDifferentCase_RemovesCorrectAttempts()
    {
        // Arrange
        var originalEmail = ValidEmail;
        var upperCaseEmail = ValidEmailUpperCase;
        var cacheKey = GetCacheKey(originalEmail);
        _cache.Set(cacheKey, 3);

        // Act
        await _service.Handle(upperCaseEmail);

        // Assert
        var exists = _cache.TryGetValue(cacheKey, out _);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_WhenEmailIsNull_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.Handle(null!));
    }

    [Fact]
    public async Task Handle_WhenEmailIsEmpty_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _service.Handle(EmptyEmail));
        Assert.Equal("email", exception.ParamName);
        Assert.Contains("Email cannot be empty or whitespace-only", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailIsWhitespace_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(async () => await _service.Handle(WhitespaceEmail));
        Assert.Equal("email", exception.ParamName);
        Assert.Contains("Email cannot be empty or whitespace-only", exception.Message);
    }

    [Fact]
    public async Task Handle_WhenEmailHasLeadingOrTrailingWhitespace_RemovesAttemptsFromCache()
    {
        // Arrange
        var email = ValidEmailWithWhitespace;
        var cacheKey = GetCacheKey(email);
        _cache.Set(cacheKey, 2);

        // Act
        await _service.Handle(email);

        // Assert
        var exists = _cache.TryGetValue(cacheKey, out _);
        Assert.False(exists);
    }

    [Fact]
    public async Task Handle_WhenMultipleEmailsExist_RemovesOnlySpecifiedEmailAttempts()
    {
        // Arrange
        var email1 = ValidEmail;
        var email2 = _faker.Internet.Email();
        var cacheKey1 = GetCacheKey(email1);
        var cacheKey2 = GetCacheKey(email2);

        _cache.Set(cacheKey1, 4);
        _cache.Set(cacheKey2, 2);

        // Act
        await _service.Handle(email1);

        // Assert
        Assert.False(_cache.TryGetValue(cacheKey1, out _));
        Assert.True(_cache.TryGetValue(cacheKey2, out _));
    }

    [Fact]
    public async Task Handle_WhenEmailContainsSpecialCharacters_RemovesAttemptsFromCache()
    {
        // Arrange - Test with email containing special characters (common in some email systems)
        var email = "user+tag@example.co.uk";
        var cacheKey = GetCacheKey(email);
        _cache.Set(cacheKey, 1);

        // Act
        await _service.Handle(email);

        // Assert
        var exists = _cache.TryGetValue(cacheKey, out _);
        Assert.False(exists);
    }

    /// <summary>
    /// Helper method to generate cache key in the same way as the service
    /// This ensures consistency in tests and avoids duplication
    /// </summary>
    /// <param name="email">Email address to generate key for</param>
