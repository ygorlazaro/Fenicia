using Fenicia.Common.Database.Abstracts;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

public class StockMovementRepository(BasicContext context) : BaseRepository<StockMovementModel>(context), IStockMovementRepository
{
    public async Task<List<StockMovementModel>> GetMovementAsync(Guid productId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken, int page, int perPage)
    {
        var query = from sm in context.StockMovements
                    where sm.ProductId == productId
                        && sm.Date >= startDate
                        && sm.Date <= endDate
                    select sm;

        return await query.Skip((page - 1) * perPage).Take(perPage).Include(sm => sm.Product).ToListAsync(cancellationToken);
    }

    public async Task<List<StockMovementModel>> GetMovementAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var query = from sm in context.StockMovements
                    where sm.Date >= startDate
                          && sm.Date <= endDate
                    select sm;

        return await query.Skip((page - 1) * perPage).Take(perPage).Include(sm => sm.Product).ToListAsync(cancellationToken);
    }
}
