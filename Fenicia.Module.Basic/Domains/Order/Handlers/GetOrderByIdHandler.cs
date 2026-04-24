using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Order.Queries;
using Fenicia.Module.Basic.Domains.Order.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

public class GetOrderByIdHandler(DefaultContext db)
{
    public async Task<GetOrderByIdResponse?> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await db.BasicOrders
            .Include(o => o.Customer).ThenInclude(c => c.Person)
            .Include(o => o.Details).ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(o => o.Id == query.Id, ct);

        if (order is null)
        {
            return null;
        }

        return new GetOrderByIdResponse(
            order.Id,
            order.OrderNumber,
            order.UserId,
            order.CustomerId,
            order.Customer?.Person?.Name ?? "Unknown",
            order.TotalAmount,
            order.DiscountAmount,
            order.TotalQuantity,
            order.SaleDate,
            order.Status.ToString(),
            order.PaymentMethod,
            order.Notes,
            order.EmployeeId);
    }
}
