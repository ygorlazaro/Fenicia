using MediatR;

namespace Fenicia.Auth.Domains.RefreshToken.Commands;

public sealed record InvalidateRefreshTokenCommand(string RefreshToken) : IRequest;
