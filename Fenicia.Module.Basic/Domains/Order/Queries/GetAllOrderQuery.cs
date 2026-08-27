using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Order.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Order.Queries;

public record GetAllOrderQuery(int Page = 1, int PerPage = 10) : IRequest<Pagination<List<GetAllOrderResponse>>>;
