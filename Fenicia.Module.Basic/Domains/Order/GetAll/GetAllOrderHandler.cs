using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.GetAll;

public class GetAllOrderHandler(DefaultContext context)
{
    public async Task<Pagination<List<GetAllOrderResponse>>> Handle(GetAllOrderQuery query, CancellationToken ct)
    {
        var total = await context.BasicOrders.CountAsync(ct);

        var orders = await context.BasicOrders
            .Include(o => o.CustomerModel)
            .ThenInclude(c => c.PersonModel)
            .Include(o => o.Details)
            .OrderByDescending(o => o.SaleDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = orders.Select(o => new GetAllOrderResponse(
            o.Id,
            o.UserId,
            o.CustomerId,
            o.CustomerModel?.PersonModel?.Name ?? "Unknown",
            o.TotalAmount,
            o.SaleDate,
            o.Status.ToString(),
            o.Details.Count)).ToList();

        return new Pagination<List<GetAllOrderResponse>>(response, total, query.Page, query.PerPage);
    }
}
