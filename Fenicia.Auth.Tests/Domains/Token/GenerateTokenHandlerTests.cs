using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.IncrementAttempts;
using Fenicia.Auth.Domains.LoginAttempt.LoginAttempt;
using Fenicia.Auth.Domains.Security.VerifyPassword;
using Fenicia.Auth.Domains.Token.GenerateToken;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.Token;

public class GenerateTokenHandlerTests : IDisposable
{
    public GenerateTokenHandlerTests()
    {
        this.cache = new MemoryCache(new MemoryCacheOptions());
        var loginAttemptHandler = new LoginAttemptHandler(this.cache);
        var incrementAttemptsHandler = new IncrementAttempts(this.cache);
        var verifyPasswordHandler = new VerifyPasswordHandler();

        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new DefaultContext(options, new TestCompanyContext());
        var getByEmailHandler = new GetByEmailHandler(this.context);
        this.handler = new GenerateTokenHandler(
            loginAttemptHandler,
            getByEmailHandler,
            incrementAttemptsHandler,
            verifyPasswordHandler);
        this.faker = new Faker();
    }

    public void Dispose()
    {
        this.context.Dispose();
        this.cache.Dispose();
        
        GC.SuppressFinalize(this);
    }

    private readonly MemoryCache cache;
    private readonly GenerateTokenHandler handler;
    private readonly DefaultContext context;
    private readonly Faker faker;

    [Fact]
    public async Task Handle_WhenTooManyAttempts_ThrowsPermissionDeniedException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var query = new GenerateTokenQuery(email, this.faker.Internet.Password());
        SetupCacheAttempts(email, 5);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.Equal("Too many login attempts. Please try again later.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsPermissionDeniedException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var query = new GenerateTokenQuery(email, this.faker.Internet.Password());
        SetupCacheAttempts(email, 2);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
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

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(query.Email, result.Email);
    }

    [Fact]
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

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
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

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act
        await Record.ExceptionAsync(async () => await this.handler.Handle(query, CancellationToken.None));
    }

    [Fact]
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

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        _ = await Record.ExceptionAsync(async () =>
            await this.handler.Handle(query, CancellationToken.None));

        // Verify increment was called by checking cache
        var key = $"login-attempt:{query.Email.ToLower()}";
        Assert.True(this.cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Handle_WhenEmailIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var email = this.faker.Internet.Email();
        var query = new GenerateTokenQuery(string.Empty, this.faker.Internet.Password());
        SetupCacheAttempts(email, 0);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await this.handler.Handle(query, CancellationToken.None));
    }

    [Fact]
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

        this.context.AuthUsers.Add(user);
        await this.context.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () =>
            await this.handler.Handle(query, CancellationToken.None));
        Assert.Contains("Password", ex.Message);
    }

    private void SetupCacheAttempts(string email, int attempts)
    {
        var key = $"login-attempt:{email.ToLower()}";
        if (attempts > 0)
        {
            this.cache.Set(key, attempts);
        }
    }
}
