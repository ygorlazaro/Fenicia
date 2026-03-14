using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.StockMovement.Queries;
using Fenicia.Module.Basic.Domains.StockMovement.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Handlers;

/// <summary>
/// Handler responsible for retrieving stock movements filtered by date range.
/// Returns a paginated list of movements with related entity information.
/// </summary>
public class GetStockMovementHandler(DefaultContext context)
{
    /// <summary>
    /// Retrieves stock movements by date range with pagination.
    /// </summary>
    /// <param name="query">The query containing date range and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of stock movements.</returns>
    public async Task<List<GetStockMovementResponse>> Handle(GetStockMovementQuery query, CancellationToken ct)
    {
        var request = from m in context.BasicStockMovements
                      join c in context.BasicCustomers on m.CustomerId equals c.Id into customers
                      from c in customers.DefaultIfEmpty()
                      join s in context.BasicSuppliers on m.SupplierId equals s.Id into suppliers
                      from s in suppliers.DefaultIfEmpty()
                      join e in context.BasicEmployees on m.EmployeeId equals e.Id into employees
                      from e in employees.DefaultIfEmpty()
                      where m.Date >= query.StartDate && m.Date <= query.EndDate
                    select new GetStockMovementResponse(
                        m.Id,
                        m.ProductId,
                        m.Product.Name,
                        m.Quantity,
                        m.Date,
                        m.Price,
                        m.Type,
                        m.CustomerId,
                        c != null && c.Person != null ? c.Person.Name : null,
                        m.SupplierId,
                        s != null && s.Person != null ? s.Person.Name : null,
                        m.EmployeeId,
                        e != null && e.Person != null ? e.Person.Name : null,
                        m.OrderId,
                        m.Reason);
        
        return await request
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
