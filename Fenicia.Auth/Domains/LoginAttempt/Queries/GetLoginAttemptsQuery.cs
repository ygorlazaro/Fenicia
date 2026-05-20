using MediatR;

namespace Fenicia.Auth.Domains.LoginAttempt.Queries;

/// <summary>
///     Query to retrieve failed login attempts for a specific email.
/// </summary>
public sealed record GetLoginAttemptsQuery(string Email) : IRequest<int>;
