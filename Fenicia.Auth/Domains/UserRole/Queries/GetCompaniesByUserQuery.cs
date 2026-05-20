using Fenicia.Auth.Domains.UserRole.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.UserRole.Queries;

public record GetCompaniesByUserQuery(Guid UserId) : IRequest<List<UserRoleResponse>>;
