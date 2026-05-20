using MediatR;

namespace Fenicia.Auth.Domains.LoginAttempt.Commands;

/// <summary>
///     Command to increment failed login attempts for a specific email.
/// </summary>
public sealed record IncrementLoginAttemptsCommand(string Email) : IRequest;
