using System.IdentityModel.Tokens.Jwt;

using Bogus;

using Fenicia.Auth.Domains.Token;
using Fenicia.Common.Database.Responses;

using Microsoft.Extensions.Configuration;

using Moq;

namespace Fenicia.Auth.Tests.Services;

public class TokenServiceTests
{
    private Mock<IConfiguration> configurationMock = null!;
    private Faker faker = null!;
    private string jwtSecret = null!;
    private TokenService sut = null!;

    [SetUp]
    public void Setup()
    {
        faker = new Faker();
        jwtSecret = faker.Random.AlphaNumeric(length: 32);

        configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["Jwt:Secret"]).Returns(jwtSecret);

        sut = new TokenService();
    }

    [Test]
    public void GenerateTokenWithValidInputsReturnsValidToken()
    {
        // Arrange
        var user = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Name.FullName()
        };

        // Act
        var result = sut.GenerateToken(user);

        // Assert
        Assert.That(result.Data, Is.Not.Null);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data);

        using (Assert.EnterMultipleScope())
        {
            // Verify only claims that TokenService actually produces
            Assert.That(token.Claims.First(c => c.Type == "userId").Value, Is.EqualTo(user.Id.ToString()));
            Assert.That(token.Claims.First(c => c.Type == "unique_name").Value, Is.EqualTo(user.Name));
            Assert.That(token.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Jti), Is.True);
        }
    }

    [Test]
    public void GenerateTokenValidatesTokenExpiration()
    {
        // Arrange
        var user = new UserResponse
        {
            Id = Guid.NewGuid(),
            Email = faker.Internet.Email(),
            Name = faker.Name.FullName()
        };

        // Act
        var result = sut.GenerateToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result.Data);

        var expectedExpiration = DateTime.UtcNow.AddHours(value: 3);
        Assert.That(token.ValidTo, Is.EqualTo(expectedExpiration).Within(TimeSpan.FromSeconds(seconds: 5)));
    }
}
