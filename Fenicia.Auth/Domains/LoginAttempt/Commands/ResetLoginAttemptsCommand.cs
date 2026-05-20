using MediatR;

namespace Fenicia.Auth.Domains.LoginAttempt.Commands;

/// <summary>
///     Command to reset failed login attempts for a specific email.
/// </summary>
public sealed record ResetLoginAttemptsCommand(string Email) : IRequest;
