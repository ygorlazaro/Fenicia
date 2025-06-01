using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Fenicia.Auth.Contexts.Models;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Enums;

using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Auth.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(UserModel user, string[] roles, Guid companyId, List<ModuleType> modules)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ??
                                          throw new InvalidOperationException(TextConstants.InvalidJwtSecret));

        var authClaims = new List<Claim>
        {
            new ("userId", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new("companyId", companyId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        authClaims.AddRange(roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        authClaims.AddRange(modules.Select(m => new Claim("module", m.ToString())));

        if (roles.Any(r => r == "God"))
        {
            authClaims.Add(new Claim("module", "erp"));
        }

        var authSigningKey = new SymmetricSecurityKey(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(authClaims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}