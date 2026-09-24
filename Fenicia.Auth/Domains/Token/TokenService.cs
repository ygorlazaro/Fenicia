using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fenicia.Auth.Domains.LoginAttempt.Interfaces;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Auth.Domains.Security.Interfaces;
using Fenicia.Auth.Domains.Token.Interfaces;
using Fenicia.Auth.Domains.User.Interfaces;
using Fenicia.Common.DTOs.Auth.Token;
using Fenicia.Common.Exceptions;
using Fenicia.Common.Localization;
using Microsoft.IdentityModel.Tokens;

namespace Fenicia.Auth.Domains.Token;

/// <summary>
/// Service implementation for generating and managing JWT authentication tokens.
/// </summary>
/// <param name="configuration">The application configuration.</param>
/// <param name="loginAttemptService">The login attempt service.</param>
/// <param name="userService">The user service.</param>
/// <param name="securityService">The security service.</param>
/// <param name="refreshTokenService">The refresh token service.</param>
public sealed class TokenService(
    IConfiguration configuration,
    ILoginAttemptService loginAttemptService,
    IUserService userService,
    ISecurityService securityService,
    IRefreshTokenService refreshTokenService) : ITokenService
{
    /// <summary>
    /// Generates a JWT token for the user based on the provided credentials.
    /// </summary>
    /// <param name="request">The token request containing email and password.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the token response.</returns>
    /// <exception cref="PermissionDeniedException">Thrown when credentials are invalid or too many attempts.</exception>
    /// <exception cref="BadRequestException">Thrown when email or password is empty.</exception>
    public async Task<TokenResponse> GenerateAsync(
        TokenRequest request,
        CancellationToken cancellationToken = default)
    {
        var attempts = ValidateAttempts(request);
        var user = await userService.FirstByEmailOrDefaultAsync(request.Email, cancellationToken);

        if (user is null)
        {
            await loginAttemptService.IncrementAsync(request.Email);
            await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), cancellationToken);

            throw new PermissionDeniedException(ExceptionMessages.InvalidUsernameOrPassword);
        }

        var isValidPassword = securityService.Verify(request.Password, user.Password);

        if (isValidPassword)
        {
            loginAttemptService.Reset(request.Email);

            var companies = await userService.GetCompaniesAsync(user.Id, cancellationToken);
            var companyId = companies.Count == 1 ? companies[0].CompanyId : Guid.Empty;
            var roles = user.UsersRoles.Select(ur => ur.Role.Name).ToList();

            var tokenResponse = new TokenResponse(user.Id, user.Name, user.Email, companyId, roles);
            var refreshToken = await refreshTokenService.GenerateAsync(user.Id, cancellationToken);
            var stringToken = GenerateString(tokenResponse);

            tokenResponse.Token = stringToken;
            tokenResponse.RefreshToken = refreshToken;

            return tokenResponse;
        }

        await loginAttemptService.IncrementAsync(request.Email);
        await Task.Delay(TimeSpan.FromSeconds(Math.Min(attempts, 5)), cancellationToken);

        throw new PermissionDeniedException(ExceptionMessages.InvalidUsernameOrPassword);
    }

    /// <summary>
    /// Generates a JWT token string from a token response.
    /// </summary>
    /// <param name="user">The token response containing user information.</param>
    /// <returns>The generated JWT token string.</returns>
    public string GenerateString(TokenResponse user)
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

    /// <summary>
    /// Generates the claims for the JWT token based on user information.
    /// </summary>
    /// <param name="user">The token response containing user information.</param>
    /// <returns>A list of claims for the JWT token.</returns>
    private static List<Claim> GenerateClaims(TokenResponse user)
    {
        var authClaims = new List<Claim>
        {
            new("userId", user.UserId.ToString()), new("email", user.Email), new("unique_name", user.Name),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var companyIdProp = user.GetType().GetProperty("CompanyId");
        if (companyIdProp != null)
        {
            var companyIdValue = companyIdProp.GetValue(user);
            if (companyIdValue is Guid companyIdGuid && companyIdGuid != Guid.Empty)
            {
                authClaims.Add(new Claim("companyId", companyIdGuid.ToString()));
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

        authClaims.AddRange(
            modulesList.Where(m => !string.IsNullOrEmpty(m)).Select(m => new Claim("module", m ?? string.Empty)));

        return authClaims;
    }

    /// <summary>
    /// Validates the login attempts for the given request.
    /// </summary>
    /// <param name="request">The token request containing email and password.</param>
    /// <returns>The number of login attempts.</returns>
    /// <exception cref="BadRequestException">Thrown when email or password is empty.</exception>
    /// <exception cref="PermissionDeniedException">Thrown when there are too many login attempts.</exception>
    private int ValidateAttempts(TokenRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new BadRequestException(ExceptionMessages.PasswordCannotBeNullOrEmpty);
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            throw new BadRequestException(ExceptionMessages.InvalidRequest);
        }

        var attempts = loginAttemptService.GetAttempts(request.Email);

        return attempts switch
        {
            >= 5 => throw new PermissionDeniedException(ExceptionMessages.TooManyLoginAttempts),
            _ => attempts
        };
    }
}
