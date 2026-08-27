using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Position.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Queries;

public record GetAllPositionQuery(int Page = 1, int PerPage = 10) : IRequest<Pagination<List<GetAllPositionResponse>>>;
