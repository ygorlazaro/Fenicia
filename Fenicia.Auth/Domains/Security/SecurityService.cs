namespace Fenicia.Auth.Domains.Security;

using System.Net;

using BCrypt.Net;

using Common;

public class SecurityService : ISecurityService
{
    private readonly ILogger<SecurityService> logger;

    public SecurityService(ILogger<SecurityService> logger)
    {
        this.logger = logger;
    }

    public ApiResponse<string> HashPassword(string password)
    {
        try
        {
            if (string.IsNullOrEmpty(password))
            {
                this.logger.LogError("Attempt to hash null or empty password");
                throw new ArgumentException(TextConstants.InvalidPassword);
            }

            var hashed = BCrypt.HashPassword(password, BCrypt.GenerateSalt(workFactor: 12));

            if (hashed == null)
            {
                this.logger.LogError("BCrypt failed to generate hash");
                throw new Exception("Error hashing password");
            }

            this.logger.LogInformation("Password hashed successfully");
            return new ApiResponse<string>(hashed);
        }
        catch (Exception ex) when (ex is not ArgumentException)
        {
            this.logger.LogError(ex, "Unexpected error while hashing password");
            throw;
        }
    }

    public ApiResponse<bool> VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                this.logger.LogError("Attempt to verify with null or empty password/hash");
                throw new ArgumentException(TextConstants.InvalidPassword);
            }

            var result = BCrypt.Verify(password, hashedPassword);

            this.logger.LogInformation("Password verification completed");
            return new ApiResponse<bool>(result);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error during password verification");
            return new ApiResponse<bool>(data: false, HttpStatusCode.InternalServerError);
        }
    }
}
