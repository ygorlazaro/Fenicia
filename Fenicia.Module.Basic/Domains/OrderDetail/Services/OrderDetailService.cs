using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs.Queries;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail;

public class OrderDetailService(DefaultContext db)
{
    public async Task<List<GetOrderDetailsByOrderIdResponse>> GetByOrderIdAsync(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
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
