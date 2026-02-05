using System.IdentityModel.Tokens.Jwt;

using Fenicia.Auth.Domains.Token;
using Fenicia.Common.Database.Responses;

namespace Fenicia.Auth.Tests.Services
{
    public class TokenServiceTests
    {
        private TokenService sut = null!;

        public class ExtendedUserResponse : UserResponse
        {
            public Guid CompanyId
            {
                get; set;
            }
            public List<string>? Roles
            {
                get; set;
            }
            public List<string>? Modules
            {
                get; set;
            }
        }

        [SetUp]
        public void Setup()
        {
            sut = new TokenService();
        }

        [Test]
        public void GenerateToken_IncludesBasicClaims()
        {
            var user = new UserResponse { Id = Guid.NewGuid(), Email = "t@test", Name = "Test" };

            var token = sut.GenerateToken(user);

            var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = parsed.Claims.ToList();

            Assert.That(claims.Any(c => c.Type == "userId" && c.Value == user.Id.ToString()));
            Assert.That(claims.Any(c => c.Type == "email" && c.Value == user.Email));
            Assert.That(claims.Any(c => c.Type == "unique_name" && c.Value == user.Name));
            Assert.That(claims.Any(c => c.Type == JwtRegisteredClaimNames.Jti));
        }

        [Test]
        public void GenerateToken_IncludesCompanyId_WhenPresent()
        {
            var user = new ExtendedUserResponse { Id = Guid.NewGuid(), Email = "a@b", Name = "N", CompanyId = Guid.NewGuid() };

            var token = sut.GenerateToken(user);
            var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.That(parsed.Claims.Any(c => c.Type == "companyId" && c.Value == user.CompanyId.ToString()));
        }

        [Test]
        public void GenerateToken_AddsRolesAndModulesAndGodAddsErp()
        {
            var user = new ExtendedUserResponse
            {
                Id = Guid.NewGuid(),
                Email = "r@r",
                Name = "R",
                Roles = new List<string> { "User", "God" },
                Modules = new List<string> { "sales" }
            };

            var token = sut.GenerateToken(user);
            var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var claims = parsed.Claims.ToList();

            // roles
            Assert.That(claims.Count(c => c.Type == "role") >= 2);
            Assert.That(claims.Any(c => c.Type == "role" && c.Value == "God"));

            // modules contains original and added 'erp'
            Assert.That(claims.Any(c => c.Type == "module" && c.Value == "sales"));
            Assert.That(claims.Any(c => c.Type == "module" && c.Value == "erp"));
        }

        [Test]
        public void GenerateToken_NoModulesProperty_DoesNotAddModuleClaims()
        {
            var user = new UserResponse { Id = Guid.NewGuid(), Email = "nm@nm", Name = "NM" };
            var token = sut.GenerateToken(user);
            var parsed = new JwtSecurityTokenHandler().ReadJwtToken(token);

            Assert.That(parsed.Claims.All(c => c.Type != "module"));
        }
    }
}
