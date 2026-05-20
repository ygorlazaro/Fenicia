using Fenicia.Auth.Domains.User.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.User.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<GetUserByIdResponse?>;
