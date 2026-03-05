using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.GetById;

public class GetOrderByIdHandler(DefaultContext context)
{
    public async Task<GetOrderByIdResponse?> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await context.BasicOrders
            .Include(o => o.CustomerModel)
            .ThenInclude(c => c.PersonModel)
            .Include(o => o.Details)
            .ThenInclude(d => d.ProductModel)
            .FirstOrDefaultAsync(o => o.Id == query.Id, ct);

        if (order is null)
        {
            return null;
        }

        var details = order.Details.Select(d => new OrderDetailResponse(
            d.Id,
            d.ProductId,
            d.ProductModel?.Name ?? "Unknown",
            d.Price,
            d.Quantity,
            d.Price * (decimal)d.Quantity)).ToList();

        return new GetOrderByIdResponse(
            order.Id,
            order.UserId,
            order.CustomerId,
            order.CustomerModel?.PersonModel?.Name ?? "Unknown",
            order.TotalAmount,
            order.SaleDate,
            order.Status.ToString(),
            details);
    }
}
