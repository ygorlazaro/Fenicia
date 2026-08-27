using System.Globalization;

using Fenicia.Auth.Domains.LoginAttempt.Commands;

using MediatR;

using Microsoft.Extensions.Caching.Memory;

namespace Fenicia.Auth.Domains.LoginAttempt.Handlers;

public class ResetLoginAttemptsHandler(IMemoryCache cache) : IRequestHandler<ResetLoginAttemptsCommand>
{

    private const string KeyPrefix = "login-attempt:";

    public Task ResetAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email);

        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty or whitespace-only");
        }

        cache.Remove(GetKey(email));
        return Task.CompletedTask;
    }

    public Task Handle(ResetLoginAttemptsCommand request, CancellationToken ct)
    {
        return ResetAsync(request.Email);
    }

    private static string GetKey(string email)
    {

        return $"{KeyPrefix}{email.Trim().ToLower(CultureInfo.InvariantCulture)}";
    }
}
