using Bogus;

using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.Token;

public class GenerateTokenHandlerTests : IDisposable
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _redisDbMock;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly TokenService _service;

    public GenerateTokenHandlerTests()
    {
        _redisMock = new Mock<IConnectionMultiplexer>();
        _redisDbMock = new Mock<IDatabase>();
        _redisMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object?>())).Returns(_redisDbMock.Object);
        var loginAttemptService = new LoginAttemptService(_redisMock.Object);

        var inMemorySettings = new Dictionary<string, string?>
        {
            {
                "Jwt:Secret", "ThisIsAVeryLongSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32Bytes"
            }
        };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        _db = new DefaultContext(options, new TestCompanyContext());
        _service = new TokenService(configuration, loginAttemptService, new UserRepository(_db), new SecurityService());
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GenerateAsync_WhenTooManyAttempts_ThrowsPermissionDeniedException()
    {
        var email = _faker.Internet.Email();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns((RedisValue)5);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
        Assert.Equal("Too many login attempts. Please try again later.", ex.Message);
    }

    [Fact]
    public async Task GenerateAsync_WhenUserDoesNotExist_ThrowsPermissionDeniedException()
    {
        var email = _faker.Internet.Email();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns((RedisValue)2);

        var ex = await Assert.ThrowsAsync<PermissionDeniedException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
        Assert.Equal("Invalid username or password.", ex.Message);
    }

    [Fact]
    public async Task GenerateAsync_WhenPasswordIsValid_ReturnsGenerateTokenResponse()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns(RedisValue.Null);

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
    public async Task GenerateAsync_WhenPasswordIsInvalid_ThrowsPermissionDeniedException()
    {
        var email = _faker.Internet.Email();
        var correctPassword = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns((RedisValue)2);

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
    public async Task GenerateAsync_WhenAttemptsAreBelowThreshold_AllowsAuthentication()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, password);
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns((RedisValue)4);

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
    public async Task GenerateAsync_WhenAuthenticationFails_IncrementsAttempts()
    {
        var email = _faker.Internet.Email();
        var correctPassword = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, _faker.Internet.Password());
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns(RedisValue.Null);

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

        _redisDbMock.Verify(x => x.StringSetAsync(key, 1, TimeSpan.FromMinutes(15), When.Always, CommandFlags.None), Times.Once);
    }

    [Fact]
    public async Task GenerateAsync_WhenEmailIsEmpty_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var query = new GenerateTokenQuery(string.Empty, _faker.Internet.Password());
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns(RedisValue.Null);

        await Assert.ThrowsAsync<InvalidRequestException>(async () => await _service.GenerateAsync(query, CancellationToken.None));
    }

    [Fact]
    public async Task GenerateAsync_WhenPasswordIsEmpty_ThrowsArgumentException()
    {
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password();
        var query = new GenerateTokenQuery(email, string.Empty);
        var key = $"login-attempt:{email.ToLower()}";

        _redisDbMock.Setup(x => x.StringGet(key, CommandFlags.None)).Returns(RedisValue.Null);

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
}
