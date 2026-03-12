using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.StockMovement.Queries;
using Fenicia.Module.Basic.Domains.StockMovement.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.StockMovement.Handlers;

public class GetStockMovementHandler(DefaultContext context)
{
    public async Task<List<GetStockMovementResponse>> Handle(GetStockMovementQuery query, CancellationToken ct)
    {
        var request = from m in context.BasicStockMovements
                      join c in context.BasicCustomers on m.CustomerId equals c.Id
                      join s in context.BasicSuppliers on  m.SupplierId equals s.Id
                      join e  in context.BasicEmployees on  m.EmployeeId equals e.Id
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
                        c.Person.Name,
                        m.SupplierId,
                        s.Person.Name,
                        m.EmployeeId,
                        e.Person.Name,
                        m.OrderId,
                        m.Reason);
        
        return await request
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
