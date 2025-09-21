namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Domains.RefreshToken;

public class RefreshTokenRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private Faker faker;
    private DbContextOptions<AuthContext> options;
    private RefreshTokenRepository sut;

    [SetUp]
    public void Setup()
    {
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        this.context = new AuthContext(this.options);
        this.sut = new RefreshTokenRepository(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task Add_SavesRefreshToken()
    {
        // Arrange
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = this.faker.Random.Hash(),
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        // Act
        this.sut.Add(refreshToken);
        await this.sut.SaveChangesAsync(this.cancellationToken);

        // Assert
        var savedToken = await this.context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == refreshToken.Id, this.cancellationToken);
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
        var token = this.faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        await this.context.RefreshTokens.AddAsync(refreshToken, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.ValidateTokenAsync(userId, token, this.cancellationToken);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForExpiredToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = this.faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.UtcNow.AddDays(value: -1), // Expired
            IsActive = true
        };

        await this.context.RefreshTokens.AddAsync(refreshToken, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.ValidateTokenAsync(userId, token, this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForInactiveToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var token = this.faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = false
        };

        await this.context.RefreshTokens.AddAsync(refreshToken, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.ValidateTokenAsync(userId, token, this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForWrongUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();
        var token = this.faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        await this.context.RefreshTokens.AddAsync(refreshToken, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        var result = await this.sut.ValidateTokenAsync(wrongUserId, token, this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public async Task InvalidateRefreshTokenAsync_DeactivatesToken()
    {
        // Arrange
        var token = this.faker.Random.Hash();
        var refreshToken = new RefreshTokenModel
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Token = token,
            ExpirationDate = DateTime.UtcNow.AddDays(value: 7),
            IsActive = true
        };

        await this.context.RefreshTokens.AddAsync(refreshToken, this.cancellationToken);
        await this.context.SaveChangesAsync(this.cancellationToken);

        // Act
        await this.sut.InvalidateRefreshTokenAsync(token, this.cancellationToken);

        // Assert
        var updatedToken = await this.context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == refreshToken.Id, this.cancellationToken);
        Assert.That(updatedToken, Is.Not.Null);
        Assert.That(updatedToken.IsActive, Is.False);
    }

    [Test]
    public void InvalidateRefreshTokenAsync_HandlesNonExistentToken()
    {
        // Arrange
        var nonExistentToken = this.faker.Random.Hash();

        // Act & Assert
        Assert.DoesNotThrowAsync(() => this.sut.InvalidateRefreshTokenAsync(nonExistentToken, this.cancellationToken));
    }

    [Test]
    public async Task ValidateTokenAsync_ReturnsFalse_ForNonExistentToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var nonExistentToken = this.faker.Random.Hash();

        // Act
        var result = await this.sut.ValidateTokenAsync(userId, nonExistentToken, this.cancellationToken);

        // Assert
        Assert.That(result, Is.False);
    }
}
