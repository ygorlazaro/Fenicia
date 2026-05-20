using MediatR;

namespace Fenicia.Auth.Domains.RefreshToken.Commands;

public sealed record GenerateRefreshTokenCommand(Guid UserId) : IRequest<string>;
