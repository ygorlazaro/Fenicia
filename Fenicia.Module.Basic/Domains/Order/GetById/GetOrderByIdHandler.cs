using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.GetById;

public class GetOrderByIdHandler(DefaultContext context)
{
    public async Task<GetOrderByIdResponse?> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await context.BasicOrders
            .Include(o => o.Customer)
            .ThenInclude(c => c.Person)
            .Include(o => o.Details)
            .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == query.Id, ct);

        if (order is null)
        {
            return null;
        }

        var details = order.Details.Select(d => new OrderDetailResponse(
            d.Id,
            d.ProductId,
            d.Product?.Name ?? "Unknown",
            d.Price,
            d.Quantity,
            d.Price * (decimal)d.Quantity)).ToList();

        return new GetOrderByIdResponse(
            order.Id,
            order.UserId,
            order.CustomerId,
            order.Customer?.Person?.Name ?? "Unknown",
            order.TotalAmount,
            order.SaleDate,
            order.Status.ToString(),
            details);
    }
}
