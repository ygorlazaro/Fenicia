using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Fenicia.Common.API;
using Fenicia.Common.Database.Responses;

using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Auth.Domains.Token;

public class TokenService : ITokenService
{
    private readonly ConfigurationManager configuration = AppSettingsReader.GetConfiguration();

    public string GenerateToken(UserResponse user)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ?? throw new InvalidOperationException());
        var authClaims = GenerateClaims(user);
        var authSigningKey = new SymmetricSecurityKey(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(value: 3),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(authClaims)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var finalToken = tokenHandler.WriteToken(token);

        return finalToken;
    }

    private static List<Claim> GenerateClaims(UserResponse user)
    {
        var authClaims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new("email", user.Email),
            new("unique_name", user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var companyIdProp = user.GetType().GetProperty("CompanyId");
        if (companyIdProp != null)
        {
            var companyIdValue = companyIdProp.GetValue(user);
            if (companyIdValue != null && !string.IsNullOrEmpty(companyIdValue.ToString()))
            {
                authClaims.Add(new Claim("companyId", companyIdValue.ToString()!));
            }
        }

        var rolesProp = user.GetType().GetProperty("Roles");

        if (rolesProp != null && rolesProp.GetValue(user) is IEnumerable<string> rolesValue)
        {
            authClaims.AddRange(rolesValue.Where(role => !string.IsNullOrEmpty(role)).Select(role => new Claim("role", role)));
        }

        var modulesProp = user.GetType().GetProperty("Modules");

        if (modulesProp == null || modulesProp.GetValue(user) is not IEnumerable<object> modulesValue)
        {
            return authClaims;
        }

        var modulesList = modulesValue.Select(m => m.ToString()).Where(m => !string.IsNullOrEmpty(m)).ToList();

        var hasGodRole = rolesProp != null && (rolesProp.GetValue(user) as IEnumerable<string>)?.Contains("God") == true;

        if (hasGodRole && !modulesList.Contains("erp"))
        {
            modulesList.Add("erp");
        }

        authClaims.AddRange(modulesList.Where(module => !string.IsNullOrEmpty(module)).Select(module => new Claim("module", module ?? string.Empty)));

        return authClaims;
    }
}
