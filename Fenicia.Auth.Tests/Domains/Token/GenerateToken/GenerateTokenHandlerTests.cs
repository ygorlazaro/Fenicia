using Bogus;

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
    [SetUp]
    public void SetUp()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        this.loginAttemptHandler = new LoginAttemptHandler(this.cache);
        this.incrementAttemptsHandler = new IncrementAttempts(this.cache);
        this.verifyPasswordHandler = new VerifyPasswordHandler();

        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new AuthContext(options);
        this.getByEmailHandler = new GetByEmailHandler(this.context);
        this.handler = new GenerateTokenHandler(
            this.loginAttemptHandler,
            this.getByEmailHandler,
            this.incrementAttemptsHandler,
            this.verifyPasswordHandler);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
        this.cache.Dispose();
    }

    private IMemoryCache cache = null!;
    private LoginAttemptHandler loginAttemptHandler = null!;
    private GetByEmailHandler getByEmailHandler = null!;
    private IncrementAttempts incrementAttemptsHandler = null!;
    private VerifyPasswordHandler verifyPasswordHandler = null!;
    private GenerateTokenHandler handler = null!;
    private AuthContext context = null!;
    private Faker faker = null!;

    [Test]
    public void Handle_WhenTooManyAttempts_ThrowsPermissionDeniedException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var query = new GenerateTokenQuery(email, this.faker.Internet.Password());
        SetupCacheAttempts(email, 5);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.TooManyAttempts));
    }

    [Test]
    public void Handle_WhenUserDoesNotExist_ThrowsPermissionDeniedException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var query = new GenerateTokenQuery(email, this.faker.Internet.Password());
        SetupCacheAttempts(email, 2);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.InvalidUsernameOrPasswordMessage));
    }

    [Test]
    public async Task Handle_WhenPasswordIsValid_ReturnsGenerateTokenResponse()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = this.faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
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
            Assert.That(result.Name, Is.EqualTo(user.Name), "Name should match");
            Assert.That(result.Email, Is.EqualTo(query.Email), "Email should match");
        }
    }

    [Test]
    public async Task Handle_WhenPasswordIsInvalid_ThrowsPermissionDeniedException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var correctPassword = this.faker.Internet.Password();
        var query = new GenerateTokenQuery(email, this.faker.Internet.Password());
        SetupCacheAttempts(email, 2);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = this.faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Is.EqualTo(TextConstants.InvalidUsernameOrPasswordMessage));
    }

    [Test]
    public async Task Handle_WhenAttemptsAreBelowThreshold_AllowsAuthentication()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        SetupCacheAttempts(email, 4);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = this.faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
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
        var email = this.faker.Internet.Email();
        var correctPassword = this.faker.Internet.Password();
        var query = new GenerateTokenQuery(email, this.faker.Internet.Password());
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = this.faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        _ = Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));

        // Verify increment was called by checking cache
        var key = $"login-attempt:{query.Email.ToLower()}";
        using (Assert.EnterMultipleScope())
        {
            Assert.That(this.cache.TryGetValue(key, out int count), Is.True);
            Assert.That(count, Is.EqualTo(1), "Should have incremented attempts");
        }
    }

    [Test]
    public void Handle_WhenEmailIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var query = new GenerateTokenQuery(string.Empty, this.faker.Internet.Password());
        SetupCacheAttempts(email, 0);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(async () => await this.handler.Handle(query, CancellationToken.None));
    }

    [Test]
    public async Task Handle_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var password = this.faker.Internet.Password();
        var query = new GenerateTokenQuery(email, string.Empty);
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = this.faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        this.context.Users.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.That(ex?.Message, Does.Contain("Password"), "Should throw ArgumentException for empty password");
    }

    private void SetupCacheAttempts(string email, int attempts)
    {
        var key = $"login-attempt:{email.ToLower()}";
        if (attempts > 0) this.cache.Set(key, attempts);
    }
}