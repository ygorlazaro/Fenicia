using System.IdentityModel.Tokens.Jwt;

using Bogus;

using Fenicia.Auth.Domains.Token;
using Fenicia.Auth.Domains.Token.DTOs;
using Microsoft.Extensions.Configuration;

namespace Fenicia.Auth.Tests.Domains.Token;

public class GenerateTokenStringHandlerTests
{
    private readonly Faker _faker;

    private readonly TokenService _handler;

    public GenerateTokenStringHandlerTests()
    {
        var inMemorySettings = new Dictionary<string, string?> { { "Jwt:Secret", "ThisIsAVeryLongSecretKeyForJwtTokenGenerationThatShouldBeAtLeast32Bytes" } };

        var configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

        _handler = new TokenService(configuration, null!, null!);
        _faker = new Faker();
    }

    [Fact]
    public void Handle_WhenValidUser_ReturnsValidToken()
    {
        var user = new GenerateTokenResponse(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email());

        var token = _handler.GenerateString(user);

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void Handle_WhenValidUser_ReturnsTokenThatCanBeRead()
    {
        var userId = Guid.NewGuid();
        var user = new GenerateTokenResponse(userId, _faker.Person.FullName, _faker.Internet.Email());

        var token = _handler.GenerateString(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        Assert.NotNull(jwtToken);
    }

    [Fact]
    public void Handle_WhenValidUser_TokenContainsCorrectClaims()
    {
        var userId = Guid.NewGuid();
        var email = _faker.Internet.Email();
        var name = _faker.Person.FullName;
        var user = new GenerateTokenResponse(userId, name, email);

        var token = _handler.GenerateString(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        Assert.Equal(userId.ToString(), jwtToken.Claims.FirstOrDefault(c => c.Type == "userId")?.Value);
        Assert.Equal(email, jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value);
        Assert.Equal(name, jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value);
        Assert.NotNull(jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti));
    }

    [Fact]
    public void Handle_WhenUserHasCompanyId_TokenContainsCompanyIdClaim()
    {
        var userWithCompany = new GenerateTokenResponseWithCompany(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), Guid.NewGuid());

        var token = _handler.GenerateString(userWithCompany);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var companyIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "companyId");
        Assert.NotNull(companyIdClaim);
        Assert.Equal(userWithCompany.CompanyId.ToString(), companyIdClaim.Value);
    }

    [Fact]
    public void Handle_WhenUserHasRoles_TokenContainsRoleClaims()
    {
        var userWithRoles = new GenerateTokenResponseWithRoles(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), ["Admin", "User", "Manager"]);

        var token = _handler.GenerateString(userWithRoles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        Assert.Equal(3, roleClaims.Count);
        Assert.Contains("Admin", roleClaims.Select(c => c.Value));
        Assert.Contains("User", roleClaims.Select(c => c.Value));
        Assert.Contains("Manager", roleClaims.Select(c => c.Value));
    }

    [Fact]
    public void Handle_WhenUserHasModules_TokenContainsModuleClaims()
    {
        var userWithModules = new GenerateTokenResponseWithModules(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), ["basic", "social"]);

        var token = _handler.GenerateString(userWithModules);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var moduleClaims = jwtToken.Claims.Where(c => c.Type == "module").ToList();

        Assert.Equal(2, moduleClaims.Count);
        Assert.Contains("basic", moduleClaims.Select(c => c.Value));
        Assert.Contains("social", moduleClaims.Select(c => c.Value));
    }

    [Fact]
    public void Handle_WhenTokenIsGenerated_HasExpiration()
    {
        var user = new GenerateTokenResponse(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email());

        var token = _handler.GenerateString(user);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
        Assert.NotNull(expClaim);
    }

    [Fact]
    public void Handle_WhenConfigurationSecretIsNull_ThrowsInvalidOperationException()
    {
        var badConfig = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>()).Build();

        var badHandler = new TokenService(badConfig, null!, null!);
        var user = new GenerateTokenResponse(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email());

        Assert.Throws<InvalidOperationException>(() => badHandler.GenerateString(user));
    }

    [Fact]
    public void Handle_WhenUserHasEmptyRoles_DoesNotAddEmptyClaims()
    {
        var userWithEmptyRoles = new GenerateTokenResponseWithRoles(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), ["Admin", string.Empty, null!, "User"]);

        var token = _handler.GenerateString(userWithEmptyRoles);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);
        var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").ToList();

        Assert.Equal(2, roleClaims.Count);
    }

    [Fact]
    public void Handle_WhenUserHasEmptyModules_DoesNotAddEmptyClaims()
    {
        var userWithEmptyModules = new GenerateTokenResponseWithModules(Guid.NewGuid(), _faker.Person.FullName, _faker.Internet.Email(), [string.Empty, null!, "basic"]);

        var token = _handler.GenerateString(userWithEmptyModules);

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
