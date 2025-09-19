namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Domains.RefreshToken;

public class RefreshTokenRepositoryTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private AuthContext _context;
    private Faker _faker;
    private DbContextOptions<AuthContext> _options;
    private RefreshTokenRepository _sut;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

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
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        // Act
        _sut.Add(refreshToken);
        await _sut.SaveChangesAsync(_cancellationToken);

        // Assert
        var savedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == refreshToken.Id, _cancellationToken);
        Assert.That(savedToken, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(savedToken.Token, Is.EqualTo(refreshToken.Token));
            Assert.That(savedToken.UserId, Is.EqualTo(refreshToken.UserId));
            Assert.That(savedToken.ExpirationDate, Is.EqualTo(refreshToken.ExpirationDate));
            Assert.That(savedToken.IsActive, Is.True);
        }
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
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        await _context.RefreshTokens.AddAsync(refreshToken, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.ValidateTokenAsync(userId, token, _cancellationToken);

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
            ExpirationDate = DateTime.UtcNow.AddDays(value: -1), // Expired
            IsActive = true
        };

        await _context.RefreshTokens.AddAsync(refreshToken, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.ValidateTokenAsync(userId, token, _cancellationToken);

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
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = false
        };

        await _context.RefreshTokens.AddAsync(refreshToken, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.ValidateTokenAsync(userId, token, _cancellationToken);

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
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        await _context.RefreshTokens.AddAsync(refreshToken, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        var result = await _sut.ValidateTokenAsync(wrongUserId, token, _cancellationToken);

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
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        await _context.RefreshTokens.AddAsync(refreshToken, _cancellationToken);
        await _context.SaveChangesAsync(_cancellationToken);

        // Act
        await _sut.InvalidateRefreshTokenAsync(token, _cancellationToken);

        // Assert
        var updatedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == refreshToken.Id, _cancellationToken);
        Assert.That(updatedToken, Is.Not.Null);
        Assert.That(updatedToken.IsActive, Is.False);
    }

    [Test]
    public void InvalidateRefreshTokenAsync_HandlesNonExistentToken()
    {
        // Arrange
        var nonExistentToken = _faker.Random.Hash();

        // Act & Assert
        Assert.DoesNotThrowAsync(() => _sut.InvalidateRefreshTokenAsync(nonExistentToken, _cancellationToken));
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForNonExistentToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentToken = _faker.Random.Hash();

        // Act
        var result = await _sut.ValidateTokenAsync(userId, nonExistentToken, _cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }
}
