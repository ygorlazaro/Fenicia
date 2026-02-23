using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.GetMovement;

public class GetStockMovementHandler(BasicContext context)
{
    public async Task<List<StockMovementResponse>> Handle(GetStockMovementQuery query, CancellationToken ct)
    {
        var movements = await context.StockMovements
            .Include(s => s.Product)
            .Where(s => s.Date >= query.StartDate && s.Date <= query.EndDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return movements.Select(m => new StockMovementResponse(m.Id, m.ProductId, m.Quantity, m.Date, m.Price, m.Type, m.CustomerId, m.SupplierId)).ToList();
    }
}
