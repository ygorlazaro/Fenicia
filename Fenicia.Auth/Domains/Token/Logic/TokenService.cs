using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Fenicia.Auth.Domains.User.Data;
using Fenicia.Common;
using Fenicia.Common.Enums;

using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Auth.Domains.Token.Logic;

/// <summary>
/// Service responsible for JWT token generation and management
/// </summary>
public class TokenService(IConfiguration configuration, ILogger<TokenService> logger)
    : ITokenService
{
    /// <summary>
    /// Generates a JWT token for the specified user
    /// </summary>
    /// <param name="user">User information</param>
    /// <param name="roles">User roles</param>
    /// <param name="companyId">Company identifier</param>
    /// <param name="modules">Accessible modules</param>
    /// <returns>API response containing the generated token</returns>
    public ApiResponse<string> GenerateToken(
        UserResponse user,
        string[] roles,
        Guid companyId,
        List<ModuleType> modules
    )
    {
        try
        {
            logger.LogInformation("Starting token generation for user {UserId}", user.Id);
        var key = Encoding.ASCII.GetBytes(
            configuration["Jwt:Secret"] ?? throw new InvalidOperationException()
        );

        var authClaims = GenerateClaims(user, roles, companyId, modules);
        var authSigningKey = new SymmetricSecurityKey(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(
                authSigningKey,
                SecurityAlgorithms.HmacSha256
            ),
            Subject = new ClaimsIdentity(authClaims)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        var finalToken = tokenHandler.WriteToken(token);

            logger.LogInformation("Token successfully generated for user {UserId}", user.Id);
            return new ApiResponse<string>(finalToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating token for user {UserId}", user.Id);
            throw;
        }
        finally
        {
            logger.LogDebug("Token generation process completed for user {UserId}", user.Id);
        }
    }

    /// <summary>
    /// Generates the claims for the JWT token
    /// </summary>
    /// <param name="user">User information</param>
    /// <param name="roles">User roles</param>
    /// <param name="companyId">Company identifier</param>
    /// <param name="modules">Accessible modules</param>
    /// <returns>List of claims</returns>
    private List<Claim> GenerateClaims(
        UserResponse user,
        string[] roles,
        Guid companyId,
        List<ModuleType> modules
    )
    {
        var authClaims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.Name),
            new("companyId", companyId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        authClaims.AddRange(roles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));
        authClaims.AddRange(modules.Select(m => new Claim("module", m.ToString())));

        if (roles.All(r => r != "God"))
        {
            return authClaims;
        }

        logger.LogDebug("Adding ERP module access for God role user {UserId}", user.Id);
        authClaims.Add(new Claim("module", "erp"));

        return authClaims;
    }
}
