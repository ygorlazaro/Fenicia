using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.OrderDetail.Queries;
using Fenicia.Module.Basic.Domains.OrderDetail.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Handlers;

public class GetOrderDetailsByOrderIdHandler(DefaultContext db) : IRequestHandler<GetOrderDetailsByOrderIdQuery, List<GetOrderDetailsByOrderIdResponse>>
{
    public async Task<List<GetOrderDetailsByOrderIdResponse>> Handle(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
    {
        var details = await db.BasicOrderDetails
            .Where(d => d.OrderId == query.OrderId)
            .ToListAsync(ct);

        return details.Select(d => new GetOrderDetailsByOrderIdResponse(
                d.Id,
                d.OrderId,
                d.ProductId,
                string.Empty,
                d.Price,
                d.DiscountAmount,
                d.Quantity,
                d.Subtotal))
            .ToList();
    }
}
