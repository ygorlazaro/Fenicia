namespace Fenicia.Auth.Domains.Token.Logic;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Common;
using Common.Enums;

using Microsoft.IdentityModel.Tokens;

using User.Data;

public class TokenService(IConfiguration configuration, ILogger<TokenService> logger) : ITokenService
{
    public ApiResponse<string> GenerateToken(UserResponse user, string[] roles, Guid companyId, List<ModuleType> modules)
    {
        try
        {
            logger.LogInformation(message: "Starting token generation for user {UserId}", user.Id);
            var key = Encoding.ASCII.GetBytes(configuration[key: "Jwt:Secret"] ?? throw new InvalidOperationException());

            var authClaims = GenerateClaims(user, roles, companyId, modules);
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

            logger.LogInformation(message: "Token successfully generated for user {UserId}", user.Id);
            return new ApiResponse<string>(finalToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error generating token for user {UserId}", user.Id);
            throw;
        }
        finally
        {
            logger.LogDebug(message: "Token generation process completed for user {UserId}", user.Id);
        }
    }

    private List<Claim> GenerateClaims(UserResponse user, string[] roles, Guid companyId, List<ModuleType> modules)
    {
        var authClaims = new List<Claim>
                         {
                             new(type: "userId", user.Id.ToString()),
                             new(ClaimTypes.Email, user.Email),
                             new(ClaimTypes.Name, user.Name),
                             new(type: "companyId", companyId.ToString()),
                             new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                         };

        authClaims.AddRange(roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        authClaims.AddRange(modules.Select(m => new Claim(type: "module", m.ToString())));

        if (roles.All(r => r != "God"))
        {
            return authClaims;
        }

        logger.LogDebug(message: "Adding ERP module access for God role user {UserId}", user.Id);
        authClaims.Add(new Claim(type: "module", value: "erp"));

        return authClaims;
    }
}
