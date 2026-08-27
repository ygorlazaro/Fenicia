using System.Globalization;

using Fenicia.Auth.Domains.LoginAttempt.Queries;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Handlers;

public class GetLoginAttemptsHandler(IMemoryCache cache) : IRequestHandler<GetLoginAttemptsQuery, int>
{

    private const string KeyPrefix = "login-attempt:";

    public virtual int GetAttempts(string email)
    {
        return cache.TryGetValue(GetKey(email), out int attempts) ? attempts : 0;
    }

    public Task<int> Handle(GetLoginAttemptsQuery request, CancellationToken ct)
    {
        return Task.FromResult(GetAttempts(request.Email));
    }

    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
}
