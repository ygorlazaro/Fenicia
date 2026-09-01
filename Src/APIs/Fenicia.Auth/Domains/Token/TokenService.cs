using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fenicia.Auth.Domains.LoginAttempt.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.Token.DTOs;
using Fenicia.Auth.Domains.Token.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;
using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Auth.Domains.Token;

public class TokenService(IConfiguration configuration, ILoginAttemptService loginAttemptService, IUserService userService, ISecurityService securityService) : ITokenService
{
    public TokenService()
        : this(null!, null!, null!, null!)
    {
    }

    public virtual async Task<GenerateTokenResponse> GenerateAsync(GenerateTokenQuery query, CancellationToken cancellationToken = default)
    {
        var attempts = ValidateAttempts(query);
        var user = await userService.FirstByEmailOrDefaultAsync(query.Email, cancellationToken);

        if (user is null)
        {
            await loginAttemptService.IncrementAsync(query.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), cancellationToken);

            throw new PermissionDeniedException(ExceptionMessages.InvalidUsernameOrPassword);
        }

        var isValidPassword = securityService.Verify(query.Password, user.Password);

        if (isValidPassword)
        {
            await loginAttemptService.ResetAsync(query.Email);

            return new GenerateTokenResponse(user.Id, user.Name, user.Email);
        }

        await loginAttemptService.IncrementAsync(query.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), cancellationToken);

        throw new PermissionDeniedException(ExceptionMessages.InvalidUsernameOrPassword);
    }

    public virtual string GenerateString(GenerateTokenResponse user)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"] ?? throw new InvalidOperationException());
        var authClaims = GenerateClaims(user);
        var authSigningKey = new SymmetricSecurityKey(key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
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

    private int ValidateAttempts(GenerateTokenQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.Password))
        {
            throw new InvalidRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        if (string.IsNullOrWhiteSpace(query.Email))
        {
            throw new InvalidRequestException(ExceptionMessages.InvalidRequest);
        }

        var attempts = loginAttemptService.GetAttempts(query.Email);

        return attempts switch
        {
            >= 5 => throw new PermissionDeniedException(ExceptionMessages.TooManyLoginAttempts),
            _ => attempts
        };
    }
}
