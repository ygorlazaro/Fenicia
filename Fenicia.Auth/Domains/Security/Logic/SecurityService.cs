using System.Net;

using Fenicia.Common;

namespace Fenicia.Auth.Domains.Security.Logic;

using Microsoft.Extensions.Logging;

/// <summary>
/// Service responsible for handling security-related operations such as password hashing and verification
/// </summary>
public class SecurityService : ISecurityService
{
    private readonly ILogger<SecurityService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityService"/> class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    public SecurityService(ILogger<SecurityService> logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Hashes the provided password using BCrypt algorithm
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>ApiResponse containing the hashed password</returns>
    /// <exception cref="ArgumentException">Thrown when password is null or empty</exception>
    public ApiResponse<string> HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogError("Attempt to hash null or empty password");
                throw new ArgumentException(TextConstants.InvalidPassword);
            }

            var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

            if (hashed == null)
            {
                _logger.LogError("BCrypt failed to generate hash");
                throw new Exception("Error hashing password");
            }

            _logger.LogInformation("Password hashed successfully");
            return new ApiResponse<string>(hashed);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogError(ex, "Unexpected error while hashing password");
            throw;
        }
    }

    /// <summary>
    /// Verifies if the provided password matches the hashed password
    /// </summary>
    /// <param name="password">The plain text password to verify</param>
    /// <param name="hashedPassword">The hashed password to compare against</param>
    /// <returns>ApiResponse containing the verification result</returns>
    public ApiResponse<bool> VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                _logger.LogError("Attempt to verify with null or empty password/hash");
                throw new ArgumentException(TextConstants.InvalidPassword);
            }

            var result =BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            _logger.LogInformation("Password verification completed");
            return new ApiResponse<bool>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password verification");
            return new ApiResponse<bool>(false, HttpStatusCode.InternalServerError);
        }
    }
}
