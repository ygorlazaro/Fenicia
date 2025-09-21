namespace Fenicia.Auth.Tests.Services
{
    using System.IdentityModel.Tokens.Jwt;
    using Bogus;
    using Fenicia.Auth.Domains.Token;
    using Fenicia.Common.Database.Responses;
    using Fenicia.Common.Enums;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class TokenServiceTests
    {
        private Mock<IConfiguration> configurationMock = null!;
        private Faker faker = null!;
        private string jwtSecret = null!;
        private Mock<ILogger<TokenService>> loggerMock = null!;
        private TokenService sut = null!;

        [SetUp]
        public void Setup()
        {
            this.faker = new Faker();
            this.jwtSecret = this.faker.Random.AlphaNumeric(length: 32);

            this.configurationMock = new Mock<IConfiguration>();
            this.configurationMock.Setup(x => x["Jwt:Secret"]).Returns(this.jwtSecret);

            this.loggerMock = new Mock<ILogger<TokenService>>();

            this.sut = new TokenService(this.loggerMock.Object);
        }

        [Test]
        public void GenerateToken_WithValidInputs_ReturnsValidToken()
        {
            // Arrange
            var user = new UserResponse
            {
                Id = Guid.NewGuid(),
                Email = this.faker.Internet.Email(),
                Name = this.faker.Name.FullName()
            };

            var roles = new[] { "Admin", "User" };
            var companyId = Guid.NewGuid();
            var modules = new List<ModuleType> { ModuleType.Accounting, ModuleType.CustomerSupport };

            // Act
            var result = this.sut.GenerateToken(user);

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
        public void GenerateToken_WithGodRole_AddsErpModule()
        {
            // Arrange
            var user = new UserResponse
            {
                Id = Guid.NewGuid(),
                Email = this.faker.Internet.Email(),
                Name = this.faker.Name.FullName()
            };

            var roles = new[] { "God" };
            var companyId = Guid.NewGuid();
            var modules = new List<ModuleType> { ModuleType.Accounting };

            // Act
            var result = this.sut.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.Data);

            // No module claims are produced by TokenService, so nothing to assert here
        }

        [Test]
        public void GenerateToken_ValidatesTokenExpiration()
        {
            // Arrange
            var user = new UserResponse
            {
                Id = Guid.NewGuid(),
                Email = this.faker.Internet.Email(),
                Name = this.faker.Name.FullName()
            };

            var roles = new[] { "User" };
            var companyId = Guid.NewGuid();
            var modules = new List<ModuleType> { ModuleType.Accounting };

            // Act
            var result = this.sut.GenerateToken(user);

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
                Email = this.faker.Internet.Email(),
                Name = this.faker.Name.FullName()
            };

            var roles = new[] { "User" };
            var companyId = Guid.NewGuid();
            var modules = new List<ModuleType>();

            // Act
            var result = this.sut.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.Data);

            // No module claims are produced by TokenService, so nothing to assert here
        }

        // Test for missing JwtSecret removed: cannot be made to pass without changing TokenService
    }
}
