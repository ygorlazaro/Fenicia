using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.GetMovement;

public class GetStockMovementHandler(BasicContext context)
{
    public async Task<List<GetStockMovementResponse>> Handle(GetStockMovementQuery query, CancellationToken ct)
    {
        return  await context.StockMovements
            .Select(m => new GetStockMovementResponse(m.Id, m.ProductId, m.Quantity, m.Date, m.Price, m.Type, m.CustomerId, m.SupplierId))
            .Where(s => s.Date >= query.StartDate && s.Date <= query.EndDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}