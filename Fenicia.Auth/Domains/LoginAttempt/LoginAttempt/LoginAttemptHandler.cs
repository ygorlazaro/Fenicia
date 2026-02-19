using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.LoginAttempt;

public class LoginAttemptHandler(IMemoryCache cache)
{
    private const string KeyPrefix = "login-attempt:";
    
    public int Handle(string email, CancellationToken ct)
    {
        return cache.TryGetValue(GetKey(email), out int attempts) ? attempts : 0;
    }
    
    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
        
}