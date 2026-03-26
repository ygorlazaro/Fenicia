using Bogus;

using Fenicia.Auth.Domains.LoginAttempt.Services;
using Fenicia.Auth.Domains.Security.Services;
using Fenicia.Auth.Domains.Token.Handlers;
using Fenicia.Auth.Domains.Token.Queries;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Tests.Domains.Token;

public class GenerateTokenHandlerTests : IDisposable
{
    private readonly MemoryCache cache;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GenerateTokenHandler handler;

    public GenerateTokenHandlerTests()
    {
        cache = new MemoryCache(new MemoryCacheOptions());
        var loginAttemptService = new LoginAttemptService(cache);
        var incrementAttemptsService = new IncrementAttemptsService(cache);
        var verifyPasswordService = new VerifyPasswordService();

        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        db = new DefaultContext(options, new TestCompanyContext());
        handler = new GenerateTokenHandler(db, loginAttemptService, incrementAttemptsService, verifyPasswordService);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        cache.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenTooManyAttempts_ThrowsPermissionDeniedException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var query = new GenerateTokenQuery(email, faker.Internet.Password());
        SetupCacheAttempts(email, 5);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(query, CancellationToken.None));
        Assert.Equal("Too many login attempts. Please try again later.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsPermissionDeniedException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var query = new GenerateTokenQuery(email, faker.Internet.Password());
        SetupCacheAttempts(email, 2);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(query, CancellationToken.None));
        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsValid_ReturnsGenerateTokenResponse()
    {
        // Arrange
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

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
        var email = faker.Internet.Email();
        var correctPassword = faker.Internet.Password();
        var query = new GenerateTokenQuery(email, faker.Internet.Password());
        SetupCacheAttempts(email, 2);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await handler.Handle(query, CancellationToken.None));
        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenAttemptsAreBelowThreshold_AllowsAuthentication()
    {
        // Arrange
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        SetupCacheAttempts(email, 4);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act
        await Record.ExceptionAsync(async () => await handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenAuthenticationFails_IncrementsAttempts()
    {
        // Arrange
        var email = faker.Internet.Email();
        var correctPassword = faker.Internet.Password();
        var query = new GenerateTokenQuery(email, faker.Internet.Password());
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        _ = await Record.ExceptionAsync(async () => await handler.Handle(query, CancellationToken.None));

        // Verify increment was called by checking cache
        var key = $"login-attempt:{query.Email.ToLower()}";
        Assert.True(cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Handle_WhenEmailIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var query = new GenerateTokenQuery(string.Empty, faker.Internet.Password());
        SetupCacheAttempts(email, 0);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var email = faker.Internet.Email();
        var password = faker.Internet.Password();
        var query = new GenerateTokenQuery(email, string.Empty);
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        db.AuthUsers.Add(user);
        await db.SaveChangesAsync(CancellationToken.None);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await handler.Handle(query, CancellationToken.None));
        Assert.Contains("Password", ex.Message);
    }

    private void SetupCacheAttempts(string email, int attempts)
    {
        var key = $"login-attempt:{email.ToLower()}";
        if (attempts > 0)
        {
            cache.Set(key, attempts);
        }
    }
}
