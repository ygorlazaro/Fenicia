using Fenicia.Module.Basic.Domains.OrderDetail.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Queries;

public record GetOrderDetailsByOrderIdQuery(Guid OrderId) : IRequest<List<GetOrderDetailsByOrderIdResponse>>;
