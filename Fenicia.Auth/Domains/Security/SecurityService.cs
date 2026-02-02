using System.Net;

using Fenicia.Common;

namespace Fenicia.Auth.Domains.Security;

public class SecurityService(ILogger<SecurityService> logger) : ISecurityService
{
    public ApiResponse<string> HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                logger.LogError("Attempt to hash null or empty password");

                throw new ArgumentException(TextConstants.InvalidPasswordMessage);
            }

            var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(workFactor: 12));

            if (hashed == null)
            {
                logger.LogError("BCrypt failed to generate hash");

                throw new Exception("Error hashing password");
            }

            logger.LogInformation("Password hashed successfully");

            return new ApiResponse<string>(hashed);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            logger.LogError(ex, "Unexpected error while hashing password");

            throw;
        }
    }

    public ApiResponse<bool> VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                logger.LogError("Attempt to verify with null or empty password/hash");

                throw new ArgumentException(TextConstants.InvalidPasswordMessage);
            }

            var result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            logger.LogInformation("Password verification completed");

            return new ApiResponse<bool>(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during password verification");

            return new ApiResponse<bool>(data: false, HttpStatusCode.InternalServerError);
        }
    }
}
