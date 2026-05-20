using Fenicia.Auth.Domains.UserRole.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.UserRole.Queries;

public sealed record GetUserCompaniesQuery(Guid UserId) : IRequest<List<GetUserCompaniesResponse>>;
