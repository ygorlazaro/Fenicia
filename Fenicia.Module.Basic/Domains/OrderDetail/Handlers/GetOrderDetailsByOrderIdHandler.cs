using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.OrderDetail.Queries;
using Fenicia.Module.Basic.Domains.OrderDetail.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Handlers;

public class GetOrderDetailsByOrderIdHandler(DefaultContext db)
{
    public async Task<List<GetOrderDetailsByOrderIdResponse>> Handle(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
    {
        var details = await db.BasicOrderDetails
            .Where(d => d.OrderId == query.OrderId)
            .Select(d =>  new GetOrderDetailsByOrderIdResponse(d.Id,
                d.OrderId,
                d.ProductId,
                d.Price,
                d.Quantity))
            .ToListAsync(ct);

        return details;
    }
}
