using Fenicia.Auth.Domains.User.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.User.Queries;

public record GetByEmailQuery(string Email) : IRequest<GetByEmailResponse?>;
