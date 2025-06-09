using Bogus;
using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class RefreshTokenRepositoryTests
{
    private AuthContext _context;
    private RefreshTokenRepository _sut;
    private Faker _faker;
    private DbContextOptions<AuthContext> _options;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new RefreshTokenRepository(_context);
        _faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task Add_SavesRefreshToken()
    {
        // Arrange
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = _faker.Random.Hash(),
            ExpirationDate = DateTime.Now.AddDays(7),
            IsActive = true,
        };

        // Act
        _sut.Add(refreshToken);
        await _sut.SaveChangesAsync();

        // Assert
        var savedToken = await _context.RefreshTokens.FindAsync(refreshToken.Id);
        Assert.That(savedToken, Is.Not.Null);
        Assert.That(savedToken.Token, Is.EqualTo(refreshToken.Token));
        Assert.That(savedToken.UserId, Is.EqualTo(refreshToken.UserId));
        Assert.That(savedToken.ExpirationDate, Is.EqualTo(refreshToken.ExpirationDate));
        Assert.That(savedToken.IsActive, Is.True);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsTrue_ForValidToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = _faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.Now.AddDays(7),
            IsActive = true,
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ValidateTokenAsync(userId, token);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForExpiredToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = _faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.Now.AddDays(-1), // Expired
            IsActive = true,
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ValidateTokenAsync(userId, token);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForInactiveToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = _faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.Now.AddDays(7),
            IsActive = false,
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ValidateTokenAsync(userId, token);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForWrongUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();
        var token = _faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.Now.AddDays(7),
            IsActive = true,
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.ValidateTokenAsync(wrongUserId, token);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task InvalidateRefreshTokenAsync_DeactivatesToken()
    {
        // Arrange
        var token = _faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = token,
            ExpirationDate = DateTime.Now.AddDays(7),
            IsActive = true,
        };

        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        // Act
        await _sut.InvalidateRefreshTokenAsync(token);

        // Assert
        var updatedToken = await _context.RefreshTokens.FindAsync(refreshToken.Id);
        Assert.That(updatedToken, Is.Not.Null);
        Assert.That(updatedToken.IsActive, Is.False);
    }

    [Test]
    public async Task InvalidateRefreshTokenAsync_HandlesNonExistentToken()
    {
        // Arrange
        var nonExistentToken = _faker.Random.Hash();

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _sut.InvalidateRefreshTokenAsync(nonExistentToken));
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForNonExistentToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentToken = _faker.Random.Hash();

        // Act
        var result = await _sut.ValidateTokenAsync(userId, nonExistentToken);

        // Assert
        Assert.That(result, Is.False);
    }
}
