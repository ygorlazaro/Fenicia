using System.Net;

using Fenicia.Common;

namespace Fenicia.Auth.Domains.Security;

public class SecurityService : ISecurityService
{
    public ApiResponse<string> HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException(TextConstants.InvalidPasswordMessage);
        }

        var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(workFactor: 12));

        return hashed == null ? throw new Exception("Error hashing password") : new ApiResponse<string>(hashed);
    }

    public ApiResponse<bool> VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                throw new ArgumentException(TextConstants.InvalidPasswordMessage);
            }

            var result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            return new ApiResponse<bool>(result);
        }
        catch (Exception)
        {
            return new ApiResponse<bool>(data: false, HttpStatusCode.InternalServerError);
        }
    }
}
