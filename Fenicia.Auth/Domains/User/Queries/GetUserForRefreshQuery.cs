using Fenicia.Auth.Domains.User.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.User.Queries;

public record GetUserForRefreshQuery(Guid UserId) : IRequest<GetUserForRefreshResponse>;
