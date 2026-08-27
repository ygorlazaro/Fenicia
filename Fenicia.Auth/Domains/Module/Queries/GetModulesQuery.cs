using Fenicia.Auth.Domains.Module.Responses;
using Fenicia.Common;

using MediatR;

namespace Fenicia.Auth.Domains.Module.Queries;

public sealed record GetModulesQuery(int Page = 1, int PerPage = 20) : IRequest<Pagination<List<GetModuleResponse>>>;
