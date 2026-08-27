using Fenicia.Auth.Domains.Company.Responses;
using Fenicia.Common;

using MediatR;

namespace Fenicia.Auth.Domains.Company.Queries;

public sealed record GetCompaniesByUserQuery(Guid UserId, int Page, int PerPage) : IRequest<Pagination<IEnumerable<GetCompaniesByUserResponse>>>;
