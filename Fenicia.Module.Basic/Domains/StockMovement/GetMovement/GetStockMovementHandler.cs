using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Basic;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.GetMovement;

public class GetStockMovementHandler(DefaultContext context)
{
    public async Task<List<GetStockMovementResponse>> Handle(GetStockMovementQuery query, CancellationToken ct)
    {
        var movementsQuery = context.BasicStockMovements
            .Include(m => m.Product)
            .Include(m => m.Customer)
            .ThenInclude(c => c != null ? c.PersonModel : null)
            .Include(m => m.Supplier)
            .ThenInclude(s => s != null ? s.Person : null)
            .Include(m => m.Employee)
            .ThenInclude(e => e != null ? e.PersonModel : null)
            .Include(m => m.OrderModel)
            .Where(m => m.Date >= query.StartDate && m.Date <= query.EndDate);

        return await movementsQuery
            .Select(m => new GetStockMovementResponse(
                m.Id,
                m.ProductId,
                m.Product.Name,
                m.Quantity,
                m.Date,
                m.Price,
                m.Type,
                m.CustomerId,
                m.CustomerId.HasValue ? m.Customer!.PersonModel.Name : null,
                m.SupplierId,
                m.SupplierId.HasValue ? m.Supplier!.Person.Name : null,
                m.EmployeeId,
                m.EmployeeId.HasValue ? m.Employee!.PersonModel.Name : null,
                m.OrderId,
                m.Reason))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
