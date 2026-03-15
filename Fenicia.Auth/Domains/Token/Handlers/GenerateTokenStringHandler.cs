using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Fenicia.Auth.Domains.Token.Responses;

using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Auth.Domains.Token.Handlers;

/// <summary>
///     Handler responsible for generating JWT token strings.
///     Creates signed JWT tokens with user claims.
/// </summary>
public class GenerateTokenStringHandler(IConfiguration configuration)
{
    /// <summary>
    ///     Generates a JWT token for the given user.
    /// </summary>
    /// <param name="user">User information to encode in the token.</param>
    /// <returns>JWT token string.</returns>
    /// <remarks>
    ///     Token includes:
    ///     - userId claim
    ///     - email claim
    ///     - name claim (unique_name)
    ///     - JWT ID (jti)
    ///     - companyId claim (if present)
    ///     - role claims (if present)
    ///     - module claims (if present)
    ///     Token expires in 3 hours.
    /// </remarks>
    public string Handle(GenerateTokenResponse user)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ?? throw new InvalidOperationException());
        var authClaims = GenerateClaims(user);
        var authSigningKey = new SymmetricSecurityKey(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(authSigningKey,
                SecurityAlgorithms.HmacSha256),
            Subject = new ClaimsIdentity(authClaims)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var finalToken = tokenHandler.WriteToken(token);

        return finalToken;
    }

    private static List<Claim> GenerateClaims(GenerateTokenResponse user)
    {
        var authClaims = new List<Claim> { new("userId", user.Id.ToString()), new("email", user.Email), new("unique_name", user.Name), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };

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
            authClaims.AddRange(rolesValue.Where(r => !string.IsNullOrEmpty(r)).Select(r => new Claim("role", r)));
        }

        var modulesProp = user.GetType().GetProperty("Modules");

        if (modulesProp == null || modulesProp.GetValue(user) is not IEnumerable<object?> modulesValue)
        {
            return authClaims;
        }

        var modulesList = modulesValue.Select(m => m?.ToString()).Where(m => !string.IsNullOrEmpty(m)).ToList();

        authClaims.AddRange(modulesList.Where(m => !string.IsNullOrEmpty(m)).Select(m => new Claim("module", m ?? string.Empty)));

        return authClaims;
    }
}
