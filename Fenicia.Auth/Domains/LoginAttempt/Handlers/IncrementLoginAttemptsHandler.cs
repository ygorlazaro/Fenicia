using System.Globalization;

using Fenicia.Auth.Domains.LoginAttempt.Commands;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Handlers;

public class IncrementLoginAttemptsHandler(IMemoryCache cache) : IRequestHandler<IncrementLoginAttemptsCommand>
{

    private const int ExpirationMinutes = 15;

    private const string KeyPrefix = "login-attempt:";

    public Task IncrementAsync(string email)
    {
        var key = GetKey(email);
        var current = cache.TryGetValue(key, out int count) ? count + 1 : 1;

        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ExpirationMinutes)
        };

        cache.Set(key, current, options);

        return Task.CompletedTask;
    }

    public Task Handle(IncrementLoginAttemptsCommand request, CancellationToken ct)
    {
        return IncrementAsync(request.Email);
    }

    private static string GetKey(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        return $"{KeyPrefix}{email.ToLower(CultureInfo.InvariantCulture)}";
    }
}
