using System.IdentityModel.Tokens.Jwt;

using Fenicia.Auth.Domains.Token.GenerateToken;
using Fenicia.Auth.Domains.Token.GenerateTokenString;

using Microsoft.Extensions.Configuration;

namespace Fenicia.Auth.Tests.Domains.Token.GenerateTokenString;

[TestFixture]
public class GenerateTokenStringHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "ThisIsAVeryLongSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32Bytes" }
        };

        this.configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        this.handler = new GenerateTokenStringHandler(this.configuration);
    }

    private IConfiguration configuration = null!;
    private GenerateTokenStringHandler handler = null!;

    [Test]
    public void Handle_WhenValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new GenerateTokenResponse(
            Guid.NewGuid(),
            "Test User",
            "test@example.com");

        // Act
        var token = this.handler.Handle(user);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty, "Token should not be empty");
    }

    [Test]
    public void Handle_WhenValidUser_ReturnsTokenThatCanBeRead()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new GenerateTokenResponse(
            userId,
            "Test User",
            "test@example.com");

        // Act
        var token = this.handler.Handle(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.That(jwtToken, Is.Not.Null, "Token should be readable");
    }

    [Test]
    public void Handle_WhenValidUser_TokenContainsCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var name = "Test User";
        var user = new GenerateTokenResponse(userId, name, email);

        // Act
        var token = this.handler.Handle(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value, Is.EqualTo(userId.ToString()));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value, Is.EqualTo(email));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value, Is.EqualTo(name));
            Assert.That(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti), Is.Not.Null,
                "Should have JTI claim");
        }
    }

    [Test]
    public void Handle_WhenUserHasCompanyId_TokenContainsCompanyIdClaim()
    {
        // Arrange
        var userWithCompany = new GenerateTokenResponseWithCompany(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            Guid.NewGuid());

        // Act
        var token = this.handler.Handle(userWithCompany);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var companyIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "companyId");
        Assert.That(companyIdClaim, Is.Not.Null, "Should have companyId claim");
        Assert.That(companyIdClaim?.Value, Is.EqualTo(userWithCompany.CompanyId.ToString()));
    }

    [Test]
    public void Handle_WhenUserHasRoles_TokenContainsRoleClaims()
    {
        // Arrange
        var userWithRoles = new GenerateTokenResponseWithRoles(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            ["Admin", "User", "Manager"]);

        // Act
        var token = this.handler.Handle(userWithRoles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(roleClaims, Has.Count.EqualTo(3), "Should have 3 role claims");
            Assert.That(roleClaims.Select(c => c.Value), Does.Contain("Admin"));
            Assert.That(roleClaims.Select(c => c.Value), Does.Contain("User"));
            Assert.That(roleClaims.Select(c => c.Value), Does.Contain("Manager"));
        }
    }

    [Test]
    public void Handle_WhenUserHasModules_TokenContainsModuleClaims()
    {
        // Arrange
        var userWithModules = new GenerateTokenResponseWithModules(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            ["erp", "basic", "social"]);

        // Act
        var token = this.handler.Handle(userWithModules);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").ToList();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(moduleClaims, Has.Count.EqualTo(3), "Should have 3 module claims");
            Assert.That(moduleClaims.Select(c => c.Value), Does.Contain("erp"));
            Assert.That(moduleClaims.Select(c => c.Value), Does.Contain("basic"));
            Assert.That(moduleClaims.Select(c => c.Value), Does.Contain("social"));
        }
    }

    [Test]
    public void Handle_WhenUserHasGodRole_AutoAddsErpModule()
    {
        // Arrange
        var userWithGodRole = new GenerateTokenResponseWithRolesAndModules(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            ["God", "Admin"],
            ["basic", "social"]);

        // Act
        var token = this.handler.Handle(userWithGodRole);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").Select(c => c.Value).ToList();

        Assert.That(moduleClaims, Does.Contain("erp"), "Should auto-add erp module for God role");
    }

    [Test]
    public void Handle_WhenUserHasGodRoleAndErpModule_DoesNotDuplicate()
    {
        // Arrange
        var userWithGodRole = new GenerateTokenResponseWithRolesAndModules(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            ["God"],
            ["erp", "basic"]);

        // Act
        var token = this.handler.Handle(userWithGodRole);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").Select(c => c.Value).ToList();

        var erpCount = moduleClaims.Count(m => m == "erp");
        Assert.That(erpCount, Is.EqualTo(1), "Should not duplicate erp module");
    }

    [Test]
    public void Handle_WhenTokenIsGenerated_HasExpiration()
    {
        // Arrange
        var user = new GenerateTokenResponse(
            Guid.NewGuid(),
            "Test User",
            "test@example.com");

        // Act
        var token = this.handler.Handle(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
        Assert.That(expClaim, Is.Not.Null, "Token should have expiration claim");
    }

    [Test]
    public void Handle_WhenConfigurationSecretIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var badConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var badHandler = new GenerateTokenStringHandler(badConfig);
        var user = new GenerateTokenResponse(
            Guid.NewGuid(),
            "Test User",
            "test@example.com");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => badHandler.Handle(user));
    }

    [Test]
    public void Handle_WhenUserHasEmptyRoles_DoesNotAddEmptyClaims()
    {
        // Arrange
        var userWithEmptyRoles = new GenerateTokenResponseWithRoles(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            ["Admin", "", null!, "User"]);

        // Act
        var token = this.handler.Handle(userWithEmptyRoles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        Assert.That(roleClaims, Has.Count.EqualTo(2), "Should only have non-empty roles");
    }

    [Test]
    public void Handle_WhenUserHasEmptyModules_DoesNotAddEmptyClaims()
    {
        // Arrange
        var userWithEmptyModules = new GenerateTokenResponseWithModules(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            ["erp", "", null!, "basic"]);

        // Act
        var token = this.handler.Handle(userWithEmptyModules);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").ToList();

        Assert.That(moduleClaims, Has.Count.EqualTo(2), "Should only have non-empty modules");
    }

    // Helper classes for testing properties that don't exist in base response
    private record GenerateTokenResponseWithCompany(Guid Id, string Name, string Email, Guid CompanyId)
        : GenerateTokenResponse(Id, Name, Email);

    private record GenerateTokenResponseWithRoles(Guid Id, string Name, string Email, IEnumerable<string> Roles)
        : GenerateTokenResponse(Id, Name, Email);

    private record GenerateTokenResponseWithModules(Guid Id, string Name, string Email, IEnumerable<string> Modules)
        : GenerateTokenResponse(Id, Name, Email);

    private record GenerateTokenResponseWithRolesAndModules(
        Guid Id,
        string Name,
        string Email,
        IEnumerable<string> Roles,
        IEnumerable<string> Modules)
        : GenerateTokenResponse(Id, Name, Email);
}