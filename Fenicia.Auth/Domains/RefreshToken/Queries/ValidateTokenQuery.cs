using MediatR;

namespace Fenicia.Auth.Domains.RefreshToken.Queries;

public record ValidateTokenQuery(Guid UserId, string RefreshToken) : IRequest<bool>;
