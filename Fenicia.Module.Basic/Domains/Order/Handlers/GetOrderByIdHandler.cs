using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Order.Queries;
using Fenicia.Module.Basic.Domains.Order.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

/// <summary>
///     Handler responsible for retrieving a specific order by ID with full details.
/// </summary>
public class GetOrderByIdHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves an order by ID with all details and product information.
    /// </summary>
    /// <param name="query">Query containing the order ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The order with details, or null if not found.</returns>
    public async Task<GetOrderByIdResponse?> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var order = await db.BasicOrders.Include(o => o.Customer).ThenInclude(c => c.Person).Include(o => o.Details).ThenInclude(d => d.Product).FirstOrDefaultAsync(o => o.Id == query.Id, ct);

        if (order is null)
        {
            return null;
        }

        var details = order.Details.Select(d => new OrderDetailResponse(d.Id, d.ProductId, d.Product?.Name ?? "Unknown", d.Price, d.Quantity, d.Price * (decimal)d.Quantity)).ToList();

        return new GetOrderByIdResponse(order.Id, order.UserId, order.CustomerId, order.Customer?.Person?.Name ?? "Unknown", order.TotalAmount, order.SaleDate, order.Status.ToString(), details);
    }
}