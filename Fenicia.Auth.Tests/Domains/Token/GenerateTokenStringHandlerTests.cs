using System.IdentityModel.Tokens.Jwt;

using Bogus;

using Fenicia.Auth.Domains.Token.Handlers;
using Fenicia.Auth.Domains.Token.Responses;

using Microsoft.Extensions.Configuration;

namespace Fenicia.Auth.Tests.Domains.Token;

public class GenerateTokenStringHandlerTests
{
    public GenerateTokenStringHandlerTests()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Secret", "ThisIsAVeryLongSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32Bytes" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();

        this.handler = new GenerateTokenStringHandler(configuration);
        this.faker = new Faker();
    }

    private readonly GenerateTokenStringHandler handler;
    private readonly Faker faker;

    [Fact]
    public void Handle_WhenValidUser_ReturnsValidToken()
    {
        // Arrange
        var user = new GenerateTokenResponse(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email());

        // Act
        var token = this.handler.Handle(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void Handle_WhenValidUser_ReturnsTokenThatCanBeRead()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new GenerateTokenResponse(
            userId,
            this.faker.Person.FullName,
            this.faker.Internet.Email());

        // Act
        var token = this.handler.Handle(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.NotNull(jwtToken);
    }

    [Fact]
    public void Handle_WhenValidUser_TokenContainsCorrectClaims()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = this.faker.Internet.Email();
        var name = this.faker.Person.FullName;
        var user = new GenerateTokenResponse(userId,
            name,
            email);

        // Act
        var token = this.handler.Handle(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Equal(userId.ToString(),
            jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")
                ?.Value);
        Assert.Equal(email,
            jwtToken.Claims.FirstOrDefault(c => c.Type == "email")
                ?.Value);
        Assert.Equal(name,
            jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")
                ?.Value);
        Assert.NotNull(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti));
    }

    [Fact]
    public void Handle_WhenUserHasCompanyId_TokenContainsCompanyIdClaim()
    {
        // Arrange
        var userWithCompany = new GenerateTokenResponseWithCompany(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            Guid.NewGuid());

        // Act
        var token = this.handler.Handle(userWithCompany);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var companyIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "companyId");
        Assert.NotNull(companyIdClaim);
        Assert.Equal(userWithCompany.CompanyId.ToString(),
            companyIdClaim.Value);
    }

    [Fact]
    public void Handle_WhenUserHasRoles_TokenContainsRoleClaims()
    {
        // Arrange
        var userWithRoles = new GenerateTokenResponseWithRoles(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            ["Admin", "User", "Manager"]);

        // Act
        var token = this.handler.Handle(userWithRoles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        Assert.Equal(3,
            roleClaims.Count);
        Assert.Contains("Admin",
            roleClaims.Select(c => c.Value));
        Assert.Contains("User",
            roleClaims.Select(c => c.Value));
        Assert.Contains("Manager",
            roleClaims.Select(c => c.Value));
    }

    [Fact]
    public void Handle_WhenUserHasModules_TokenContainsModuleClaims()
    {
        // Arrange
        var userWithModules = new GenerateTokenResponseWithModules(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            ["erp", "basic", "social"]);

        // Act
        var token = this.handler.Handle(userWithModules);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").ToList();

        Assert.Equal(3,
            moduleClaims.Count);
        Assert.Contains("erp",
            moduleClaims.Select(c => c.Value));
        Assert.Contains("basic",
            moduleClaims.Select(c => c.Value));
        Assert.Contains("social",
            moduleClaims.Select(c => c.Value));
    }

    [Fact]
    public void Handle_WhenUserHasGodRole_AutoAddsErpModule()
    {
        // Arrange
        var userWithGodRole = new GenerateTokenResponseWithRolesAndModules(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            ["God", "Admin"],
            ["basic", "social"]);

        // Act
        var token = this.handler.Handle(userWithGodRole);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").Select(c => c.Value).ToList();

        Assert.Contains("erp",
            moduleClaims);
    }

    [Fact]
    public void Handle_WhenUserHasGodRoleAndErpModule_DoesNotDuplicate()
    {
        // Arrange
        var userWithGodRole = new GenerateTokenResponseWithRolesAndModules(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            ["God"],
            ["erp", "basic"]);

        // Act
        var token = this.handler.Handle(userWithGodRole);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").Select(c => c.Value).ToList();

        var erpCount = moduleClaims.Count(m => m == "erp");
        Assert.Equal(1,
            erpCount);
    }

    [Fact]
    public void Handle_WhenTokenIsGenerated_HasExpiration()
    {
        // Arrange
        var user = new GenerateTokenResponse(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email());

        // Act
        var token = this.handler.Handle(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
        Assert.NotNull(expClaim);
    }

    [Fact]
    public void Handle_WhenConfigurationSecretIsNull_ThrowsInvalidOperationException()
    {
        // Arrange
        var badConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var badHandler = new GenerateTokenStringHandler(badConfig);
        var user = new GenerateTokenResponse(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email());

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => badHandler.Handle(user));
    }

    [Fact]
    public void Handle_WhenUserHasEmptyRoles_DoesNotAddEmptyClaims()
    {
        // Arrange
        var userWithEmptyRoles = new GenerateTokenResponseWithRoles(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            ["Admin", "", null!, "User"]);

        // Act
        var token = this.handler.Handle(userWithEmptyRoles);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        Assert.Equal(2,
            roleClaims.Count);
    }

    [Fact]
    public void Handle_WhenUserHasEmptyModules_DoesNotAddEmptyClaims()
    {
        // Arrange
        var userWithEmptyModules = new GenerateTokenResponseWithModules(
            Guid.NewGuid(),
            this.faker.Person.FullName,
            this.faker.Internet.Email(),
            ["erp", "", null!, "basic"]);

        // Act
        var token = this.handler.Handle(userWithEmptyModules);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").ToList();

        Assert.Equal(2,
            moduleClaims.Count);
    }

    // Helper classes for testing properties that don't exist in base response
    private record GenerateTokenResponseWithCompany(Guid Id, string Name, string Email, Guid CompanyId)
        : GenerateTokenResponse(Id,
            Name,
            Email);

    private record GenerateTokenResponseWithRoles(Guid Id, string Name, string Email, IEnumerable<string> Roles)
        : GenerateTokenResponse(Id,
            Name,
            Email);

    private record GenerateTokenResponseWithModules(Guid Id, string Name, string Email, IEnumerable<string> Modules)
        : GenerateTokenResponse(Id,
            Name,
            Email);

    private record GenerateTokenResponseWithRolesAndModules(
        Guid Id,
        string Name,
        string Email,
        IEnumerable<string> Roles,
        IEnumerable<string> Modules)
        : GenerateTokenResponse(Id,
            Name,
            Email);
}
