using Fenicia.Auth.Domains.Module.Responses;

using MediatR;

namespace Fenicia.Auth.Domains.Module.Queries;

public record GetUserModulesQuery(Guid CompanyId, Guid UserId) : IRequest<List<GetUserModulesResponse>>;
