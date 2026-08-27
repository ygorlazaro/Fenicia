using MediatR;

namespace Fenicia.Auth.Domains.LoginAttempt.Commands;

public sealed record IncrementLoginAttemptsCommand(string Email) : IRequest;
