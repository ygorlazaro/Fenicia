namespace Fenicia.Auth.Domains.Security.Logic;

using System.Net;

using BCrypt.Net;

using Common;

public class SecurityService : ISecurityService
{
    private readonly ILogger<SecurityService> _logger;

    public SecurityService(ILogger<SecurityService> logger)
    {
        _logger = logger;
    }

    public ApiResponse<string> HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogError(message: "Attempt to hash null or empty password");
                throw new ArgumentException(TextConstants.InvalidPassword);
            }

            var hashed = BCrypt.HashPassword(password, BCrypt.GenerateSalt(workFactor: 12));

            if (hashed == null)
            {
                _logger.LogError(message: "BCrypt failed to generate hash");
                throw new Exception(message: "Error hashing password");
            }

            _logger.LogInformation(message: "Password hashed successfully");
            return new ApiResponse<string>(hashed);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            _logger.LogError(ex, message: "Unexpected error while hashing password");
            throw;
        }
    }

    public ApiResponse<bool> VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                _logger.LogError(message: "Attempt to verify with null or empty password/hash");
                throw new ArgumentException(TextConstants.InvalidPassword);
            }

            var result = BCrypt.Verify(password, hashedPassword);

            _logger.LogInformation(message: "Password verification completed");
            return new ApiResponse<bool>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "Error during password verification");
            return new ApiResponse<bool>(data: false, HttpStatusCode.InternalServerError);
        }
    }
}
