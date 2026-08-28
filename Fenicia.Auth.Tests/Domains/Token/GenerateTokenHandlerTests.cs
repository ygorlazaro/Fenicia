using Bogus;

using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.DTOs;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Fenicia.Auth.Tests.Domains.Token;

public class GenerateTokenHandlerTests : IDisposable
{
    private readonly MemoryCache _cache;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly TokenService _service;

    public GenerateTokenHandlerTests()
    {
        _cache = new MemoryCache(new MemoryCacheOptions());
        var loginAttemptService = new LoginAttemptService(_cache);

        var inMemorySettings = new Dictionary<string, string?>
        {
            {
            "Jwt:Secret", "ThisIsAVeryLongSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32Bytes"
            }
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _service = new TokenService(_db, configuration, loginAttemptService);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        _cache.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WhenTooManyAttempts_ThrowsPermissionDeniedException()
    {
        var email = _faker.Internet.Email();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        SetupCacheAttempts(email, 5);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
        Assert.Equal("Too many login attempts. Please try again later.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ThrowsPermissionDeniedException()
    {
        var email = _faker.Internet.Email();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        SetupCacheAttempts(email, 2);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsValid_ReturnsGenerateTokenResponse()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = _faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GenerateAsync(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Name, result.Name);
        Assert.Equal(query.Email, result.Email);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsInvalid_ThrowsPermissionDeniedException()
    {
        var email = _faker.Internet.Email();
        var correctPassword = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        SetupCacheAttempts(email, 2);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = _faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public async Task Handle_WhenAttemptsAreBelowThreshold_AllowsAuthentication()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        SetupCacheAttempts(email, 4);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = _faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        await Record.ExceptionAsync(async () => await _service.GenerateAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenAuthenticationFails_IncrementsAttempts()
    {
        var email = _faker.Internet.Email();
        var correctPassword = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = _faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        _ = await Record.ExceptionAsync(async () => await _service.GenerateAsync(query, CancellationToken.None));

        var key = $"login-attempt:{query.Email.ToLower()}";
        Assert.True(_cache.TryGetValue(key, out int count));
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task Handle_WhenEmailIsEmpty_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var query = new GenerateTokenQuery(string.Empty, _faker.Internet.Password());
        SetupCacheAttempts(email, 0);

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, string.Empty);
        SetupCacheAttempts(email, 0);

        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            Email = query.Email,
            Name = _faker.Person.FullName,
            Password = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _db.AuthUsers.Add(user);
        await _db.SaveChangesAsync(CancellationToken.None);

        var ex = await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
        Assert.Contains("Password", ex.Message);
    }

    private void SetupCacheAttempts(string email, int attempts)
    {
        var key = $"login-attempt:{email.ToLower()}";
        if (attempts > 0)
        {
            _cache.Set(key, attempts);
        }
    }
}
