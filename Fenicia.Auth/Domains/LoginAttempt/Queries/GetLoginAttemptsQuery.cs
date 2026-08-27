using MediatR;

namespace Fenicia.Auth.Domains.LoginAttempt.Queries;

public sealed record GetLoginAttemptsQuery(string Email) : IRequest<int>;
