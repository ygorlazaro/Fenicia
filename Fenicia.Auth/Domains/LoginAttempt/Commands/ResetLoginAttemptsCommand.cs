using MediatR;

namespace Fenicia.Auth.Domains.LoginAttempt.Commands;

public sealed record ResetLoginAttemptsCommand(string Email) : IRequest;
