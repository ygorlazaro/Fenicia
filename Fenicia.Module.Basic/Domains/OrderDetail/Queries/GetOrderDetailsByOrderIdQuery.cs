using Fenicia.Module.Basic.Domains.OrderDetail.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Queries;

/// <summary>
///     Represents a query to retrieve all details for a specific order.
/// </summary>
public record GetOrderDetailsByOrderIdQuery(Guid OrderId) : IRequest<List<GetOrderDetailsByOrderIdResponse>>;
