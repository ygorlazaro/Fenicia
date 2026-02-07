using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

public class StockMovementRepository(BasicContext context) : BaseRepository<StockMovementModel>(context), IStockMovementRepository
{
    public async Task<List<StockMovementModel>> GetMovementAsync(Guid productId, DateTime startDate, DateTime endDate, CancellationToken ct, int page, int perPage)
    {
        var query = from sm in context.StockMovements
                    where sm.ProductId == productId
                        && sm.Date >= startDate
                        && sm.Date <= endDate
                    select sm;

        return await query.Skip((page - 1) * perPage).Take(perPage).Include(sm => sm.Product).ToListAsync(ct);
    }

    public async Task<List<StockMovementModel>> GetMovementAsync(DateTime startDate, DateTime endDate, CancellationToken ct, int page = 1, int perPage = 10)
    {
        var query = from sm in context.StockMovements
                    where sm.Date >= startDate
                          && sm.Date <= endDate
                    select sm;

        return await query.Skip((page - 1) * perPage).Take(perPage).Include(sm => sm.Product).ToListAsync(ct);
    }
}
