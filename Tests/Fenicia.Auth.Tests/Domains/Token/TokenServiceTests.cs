using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Bogus;

using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.LoginAttempt;
using Fenicia.Auth.Domains.Module;
using Fenicia.Auth.Domains.Role;
using Fenicia.Auth.Domains.Security;
using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Tests;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using StackExchange.Redis;

namespace Fenicia.Auth.Tests.Domains.Token;

public class TokenServiceTests : IDisposable
{
    private readonly Mock<IConnectionMultiplexer> _redisMock;
    private readonly Mock<IDatabase> _redisDbMock;
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly TokenService _service;

    public TokenServiceTests()
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
        var userRepository = new UserRepository(_db);
        var userRoleRepository = new UserRoleRepository(_db);
        var roleRepository = new RoleRepository(_db);
        var companyRepository = new CompanyRepository(_db);
        var userRoleService = new UserRoleService(userRoleRepository);
        var roleService = new RoleService(roleRepository);
        var companyService = new CompanyService(companyRepository);
        var moduleRepository = new ModuleRepository(_db);
        var moduleService = new ModuleService(moduleRepository);
        var userService = new UserService(userRepository, userRoleService, roleService, companyService, new SecurityService(), moduleService);
        _service = new TokenService(configuration, loginAttemptService, userService, new SecurityService());
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

    [Fact]
    public void GenerateString_WhenValidUser_ReturnsValidToken()
    {
        var user = new GenerateTokenResponse(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email());

        var token = _service.GenerateString(user);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateString_WhenValidUser_ReturnsTokenThatCanBeRead()
    {
        var userId = Guid.NewGuid();
        var user = new GenerateTokenResponse(userId, _faker.Person.FullName, _faker.Internet.Email());

        var token = _service.GenerateString(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.NotNull(jwtToken);
    }

    [Fact]
    public void GenerateString_WhenValidUser_TokenContainsCorrectClaims()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;
        var user = new GenerateTokenResponse(userId, name, email);

        var token = _service.GenerateString(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Equal(userId.ToString(), jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
        Assert.Equal(email, jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value);
        Assert.Equal(name, jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
        Assert.NotNull(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti));
    }

    [Fact]
    public void GenerateString_WhenUserHasCompanyId_TokenContainsCompanyIdClaim()
    {
        var userWithCompany = new GenerateTokenResponseWithCompany(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), Guid.NewGuid());

        var token = _service.GenerateString(userWithCompany);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var companyIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "companyId");
        Assert.NotNull(companyIdClaim);
        Assert.Equal(userWithCompany.CompanyId.ToString(), companyIdClaim.Value);
    }

    [Fact]
    public void GenerateString_WhenUserHasRoles_TokenContainsRoleClaims()
    {
        var userWithRoles = new GenerateTokenResponseWithRoles(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), ["Admin", "User", "Manager"]);

        var token = _service.GenerateString(userWithRoles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        Assert.Equal(3, roleClaims.Count);
        Assert.Contains("Admin", roleClaims.Select(c => c.Value));
        Assert.Contains("User", roleClaims.Select(c => c.Value));
        Assert.Contains("Manager", roleClaims.Select(c => c.Value));
    }

    [Fact]
    public void GenerateString_WhenUserHasModules_TokenContainsModuleClaims()
    {
        var userWithModules = new GenerateTokenResponseWithModules(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), ["basic", "social"]);

        var token = _service.GenerateString(userWithModules);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").ToList();

        Assert.Equal(2, moduleClaims.Count);
        Assert.Contains("basic", moduleClaims.Select(c => c.Value));
        Assert.Contains("social", moduleClaims.Select(c => c.Value));
    }

    [Fact]
    public void GenerateString_WhenTokenIsGenerated_HasExpiration()
    {
        var user = new GenerateTokenResponse(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email());

        var token = _service.GenerateString(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
        Assert.NotNull(expClaim);
    }

    [Fact]
    public void GenerateString_WhenConfigurationSecretIsNull_ThrowsInvalidOperationException()
    {
        var badConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        var badService = new TokenService(badConfig, null!, null!, new SecurityService());
        var user = new GenerateTokenResponse(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email());

        Assert.Throws<InvalidOperationException>(() => badService.GenerateString(user));
    }

    [Fact]
    public void GenerateString_WhenUserHasEmptyRoles_DoesNotAddEmptyClaims()
    {
        var userWithEmptyRoles = new GenerateTokenResponseWithRoles(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), ["Admin", string.Empty, null!, "User"]);

        var token = _service.GenerateString(userWithEmptyRoles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        Assert.Equal(2, roleClaims.Count);
    }

    [Fact]
    public void GenerateString_WhenUserHasEmptyModules_DoesNotAddEmptyClaims()
    {
        var userWithEmptyModules = new GenerateTokenResponseWithModules(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), [string.Empty, null!, "basic"]);

        var token = _service.GenerateString(userWithEmptyModules);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").ToList();

        Assert.Single(moduleClaims);
    }

    private sealed record GenerateTokenResponseWithCompany(Guid Id, string Name, string Email, Guid CompanyId) : GenerateTokenResponse(Id, Name, Email);

    private sealed record GenerateTokenResponseWithRoles(Guid Id, string Name, string Email, IEnumerable<string> Roles) : GenerateTokenResponse(Id, Name, Email);

    private sealed record GenerateTokenResponseWithModules(Guid Id, string Name, string Email, IEnumerable<string> Modules) : GenerateTokenResponse(Id, Name, Email);

    private sealed record GenerateTokenResponseWithRolesAndModules(Guid Id, string Name, string Email, IEnumerable<string> Roles, IEnumerable<string> Modules) : GenerateTokenResponse(Id, Name, Email);
}
