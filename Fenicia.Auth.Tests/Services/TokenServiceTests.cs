namespace Fenicia.Auth.Tests.Services;

using System.IdentityModel.Tokens.Jwt;

using Bogus;

using Common.Database.Responses;
using Common.Enums;

using Domains.Token;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Moq;

public class TokenServiceTests
{
    private Mock<IConfiguration> _configurationMock = null!;
    private Faker _faker = null!;
    private string _jwtSecret = null!;
    private Mock<ILogger<TokenService>> _loggerMock = null!;
    private TokenService _sut = null!;

    [SetUp]
    public void Setup()
    {
        _faker = new Faker();
        _jwtSecret = _faker.Random.AlphaNumeric(length: 32);

        _configurationMock = new Mock<IConfiguration>();
        _configurationMock.Setup(x => x["Jwt:Secret"]).Returns(_jwtSecret);

        _loggerMock = new Mock<ILogger<TokenService>>();

        _sut = new TokenService(_loggerMock.Object);
    }

    [Test]
    public void GenerateToken_WithValidInputs_ReturnsValidToken()
    {
        // Arrange
        var user = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Name.FullName()
        };

        var roles = new[] { "Admin", "User" };
        var companyId = Guid.NewGuid();
        var modules = new List<ModuleType> { ModuleType.Accounting, ModuleType.CustomerSupport };

        // Act
        var result = _sut.GenerateToken(user, roles, companyId, modules);

        // Assert
        Assert.That(result.Data, Is.Not.Null);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data);

        using (Assert.EnterMultipleScope())
        {
            // Verify standard claims
            Assert.That(token.Claims.First(c => c.Type == "userId").Value, Is.EqualTo(user.Id.ToString()));
            Assert.That(token.Claims.First(c => c.Type == "email").Value, Is.EqualTo(user.Email));
            Assert.That(token.Claims.First(c => c.Type == "unique_name").Value, Is.EqualTo(user.Name));
            Assert.That(token.Claims.First(c => c.Type == "companyId").Value, Is.EqualTo(companyId.ToString()));
            Assert.That(token.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Jti), Is.True);
        }

        // Verify roles
        var roleClaims = token.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
        Assert.That(roleClaims, Is.EquivalentTo(roles));

        // Verify modules
        var moduleClaims = token.Claims.Where(c => c.Type == "module").Select(c => c.Value).ToList();
        Assert.That(moduleClaims, Is.EquivalentTo(modules.Select(m => m.ToString())));
    }

    [Test]
    public void GenerateToken_WithGodRole_AddsErpModule()
    {
        // Arrange
        var user = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Name.FullName()
        };

        var roles = new[] { "God" };
        var companyId = Guid.NewGuid();
        var modules = new List<ModuleType> { ModuleType.Accounting };

        // Act
        var result = _sut.GenerateToken(user, roles, companyId, modules);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data);

        var moduleClaims = token.Claims.Where(c => c.Type == "module").Select(c => c.Value).ToList();
        Assert.That(moduleClaims, Does.Contain("erp"));
        Assert.That(moduleClaims, Does.Contain(ModuleType.Accounting.ToString()));
    }

    [Test]
    public void GenerateToken_ValidatesTokenExpiration()
    {
        // Arrange
        var user = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Name.FullName()
        };

        var roles = new[] { "User" };
        var companyId = Guid.NewGuid();
        var modules = new List<ModuleType> { ModuleType.Accounting };

        // Act
        var result = _sut.GenerateToken(user, roles, companyId, modules);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data);

        var expectedExpiration = DateTime.UtcNow.AddHours(value: 3);
        Assert.That(token.ValidTo, Is.EqualTo(expectedExpiration).Within(TimeSpan.FromSeconds(seconds: 5)));
    }

    [Test]
    public void GenerateToken_WithNoModules_OnlyIncludesBasicClaims()
    {
        // Arrange
        var user = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Name.FullName()
        };

        var roles = new[] { "User" };
        var companyId = Guid.NewGuid();
        var modules = new List<ModuleType>();

        // Act
        var result = _sut.GenerateToken(user, roles, companyId, modules);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data);

        var moduleClaims = token.Claims.Where(c => c.Type == "module").ToList();
        Assert.That(moduleClaims, Is.Empty);
    }

    [Test]
    public void GenerateToken_WithoutJwtSecret_ThrowsInvalidOperationException()
    {
        // Arrange
        _configurationMock.Setup(x => x["Jwt:Secret"]).Returns((string)null!);

        var user = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = _faker.Internet.Email(),
            Name = _faker.Name.FullName()
        };

        var roles = new[] { "User" };
        var companyId = Guid.NewGuid();
        var modules = new List<ModuleType> { ModuleType.Accounting };

        // Assert
        Assert.Throws<InvalidOperationException>(() => _sut.GenerateToken(user, roles, companyId, modules));
    }
}
