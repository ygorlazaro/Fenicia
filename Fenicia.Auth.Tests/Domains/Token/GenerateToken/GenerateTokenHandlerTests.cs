using Fenicia.Auth.Domains.LoginAttempt.IncrementAttempts;
using Fenicia.Auth.Domains.LoginAttempt.LoginAttempt;
using Fenicia.Auth.Domains.Security.VerifyPassword;
using Fenicia.Auth.Domains.Token.GenerateToken;
using Fenicia.Auth.Domains.User.GetByEmail;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.Token.GenerateToken;

[TestFixture]
public class GenerateTokenHandlerTests
{
    private IMemoryCache cache = null!;
    private LoginAttemptHandler loginAttemptHandler = null!;
    private GetByEmailHandler getByEmailHandler = null!;
    private IncrementAttempts incrementAttemptsHandler = null!;
    private VerifyPasswordHandler verifyPasswordHandler = null!;
    private GenerateTokenHandler handler = null!;
    private AuthContext context = null!;

    [SetUp]
    public void SetUp()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.loginAttemptHandler = new LoginAttemptHandler(this.cache);
        this.incrementAttemptsHandler = new IncrementAttempts(this.cache);
        this.verifyPasswordHandler = new VerifyPasswordHandler();

        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.getByEmailHandler = new GetByEmailHandler(this.context);
        this.handler = new GenerateTokenHandler(
            this.loginAttemptHandler,
            this.getByEmailHandler,
            this.incrementAttemptsHandler,
            this.verifyPasswordHandler);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
        this.cache.Dispose();
    }

    [Test]
    public void Handle_WhenTooManyAttempts_ThrowsPermissionDeniedException()
    {
        // Arrange
        var query = new GenerateTokenQuery("test@example.com", "password123");
        SetupCacheAttempts(5);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () => await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.TooManyAttempts));
    }

    [Test]
    public void Handle_WhenUserDoesNotExist_ThrowsPermissionDeniedException()
    {
        // Arrange
        var query = new GenerateTokenQuery("test@example.com", "password123");
        SetupCacheAttempts(2);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () => await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.InvalidUsernameOrPasswordMessage));
    }

    [Test]
    public async Task Handle_WhenPasswordIsValid_ReturnsGenerateTokenResponse()
    {
        // Arrange
        var query = new GenerateTokenQuery("test@example.com", "password123");
        SetupCacheAttempts(0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword(query.Password)
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(user.Id), "Id should match");
            Assert.That(result.Name, Is.EqualTo("Test User"), "Name should match");
            Assert.That(result.Email, Is.EqualTo(query.Email), "Email should match");
        }
    }

    [Test]
    public async Task Handle_WhenPasswordIsInvalid_ThrowsPermissionDeniedException()
    {
        // Arrange
        var query = new GenerateTokenQuery("test@example.com", "wrongpassword");
        SetupCacheAttempts(2);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () => await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.InvalidUsernameOrPasswordMessage));
    }

    [Test]
    public async Task Handle_WhenAttemptsAreBelowThreshold_AllowsAuthentication()
    {
        // Arrange
        var query = new GenerateTokenQuery("test@example.com", "password123");
        SetupCacheAttempts(4);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword(query.Password)
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        Assert.DoesNotThrowAsync(async () => await this.handler.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WhenAuthenticationFails_IncrementsAttempts()
    {
        // Arrange
        var query = new GenerateTokenQuery("test@example.com", "wrongpassword");
        SetupCacheAttempts(0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword("correctpassword")
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        _ = Assert.ThrowsAsync<PermissionDeniedException>(async () => await this.handler.Handle(query, CancellationToken.None));

        // Verify increment was called by checking cache
        var key = $"login-attempt:{query.Email.ToLower()}";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(1), "Should have incremented attempts");
        }
    }

    [Test]
    public void Handle_WhenEmailIsEmpty_ThrowsArgumentNullException()
    {
        // Arrange
        var query = new GenerateTokenQuery(string.Empty, "password123");
        SetupCacheAttempts(0);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await this.handler.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var query = new GenerateTokenQuery("test@example.com", string.Empty);
        SetupCacheAttempts(0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = "Test User",
            Password = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Does.Contain("Password"), "Should throw ArgumentException for empty password");
    }

    private void SetupCacheAttempts(int attempts)
    {
        var key = "login-attempt:test@example.com";
        if (attempts > 0)
        {
            this.cache.Set(key, attempts);
        }
    }
}
