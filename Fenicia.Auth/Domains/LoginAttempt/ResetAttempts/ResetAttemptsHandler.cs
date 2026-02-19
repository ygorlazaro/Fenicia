using System.Globalization;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.ResetAttempts;

public class ResetAttemptsHandler(IMemoryCache cache)
{
    private const string KeyPrefix = "login-attempt:";

    public Task Handle(string email, CancellationToken ct)
    {
        cache.Remove(GetKey(email));

        return Task.CompletedTask;
    }
    
    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
}