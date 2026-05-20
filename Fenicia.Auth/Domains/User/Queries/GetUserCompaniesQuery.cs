using Fenicia.Auth.Domains.UserRole.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.User.Queries;

public record GetUserCompaniesQuery(Guid UserId) : IRequest<List<GetUserCompaniesResponse>>;
