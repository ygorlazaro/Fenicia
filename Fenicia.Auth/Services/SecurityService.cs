using System.Net;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;

namespace Fenicia.Auth.Services;

public class SecurityService : ISecurityService
{
    public ServiceResponse<string> HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentException(TextConstants.InvalidPassword);
        }
        
        var hashed = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

        if (hashed == null)
        {
            throw new Exception("Error hashing password");
        }

        return new ServiceResponse<string>(hashed);
    }

    public ServiceResponse<bool> VerifyPassword(string password, string hashedPassword)
    {
        try
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
            {
                throw new Exception();
            }
            
            var result = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

            return new ServiceResponse<bool>(result);
        }
        catch
        {
            return new ServiceResponse<bool>(false, HttpStatusCode.InternalServerError);
        }
    }
}