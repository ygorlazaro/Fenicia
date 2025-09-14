namespace Fenicia.Auth.Domains.Token;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Common;
using Common.Enums;

using Common.Database.Responses;

using Microsoft.IdentityModel.Tokens;
using Fenicia.Common.Api;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;

    public TokenService(ILogger<TokenService> logger)
    {
        _configuration = AppSettingsReader.GetConfiguration();
        _logger = logger;
    }

    public ApiResponse<string> GenerateToken(UserResponse user, string[] roles, Guid companyId, List<ModuleType> modules)
    {
        try
        {
            _logger.LogInformation("Starting token generation for user {UserId}", user.Id);
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Secret"] ?? throw new InvalidOperationException());

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

            _logger.LogInformation("Token successfully generated for user {UserId}", user.Id);
            return new ApiResponse<string>(finalToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user {UserId}", user.Id);
            throw;
        }
        finally
        {
            _logger.LogDebug("Token generation process completed for user {UserId}", user.Id);
        }
    }

    private List<Claim> GenerateClaims(UserResponse user, string[] roles, Guid companyId, List<ModuleType> modules)
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

        _logger.LogDebug("Adding ERP module access for God role user {UserId}", user.Id);
        authClaims.Add(new Claim("module", "erp"));

        return authClaims;
    }
}
