using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

public class GetOrderDetailsByOrderIdHandler(DefaultContext context)
{
    public async Task<List<GetOrderDetailsByOrderIdResponse>> Handle(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
    {
        var details = await context.BasicOrderDetails
            .Where(d => d.OrderId == query.OrderId)
            .ToListAsync(ct);

        return details.Select(d => new GetOrderDetailsByOrderIdResponse(d.Id, d.OrderId, d.ProductId, d.Price, d.Quantity)).ToList();
    }
}
