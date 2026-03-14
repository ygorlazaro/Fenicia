using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Order.Queries;
using Fenicia.Module.Basic.Domains.Order.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

/// <summary>
/// Handler responsible for retrieving all orders with pagination.
/// Returns orders with customer and employee information.
/// </summary>
public class GetAllOrderHandler(DefaultContext db)
{
    /// <summary>
    /// Retrieves paginated orders with related customer and employee data.
    /// </summary>
    /// <param name="query">Pagination query parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated list of orders.</returns>
    public async Task<Pagination<List<GetAllOrderResponse>>> Handle(GetAllOrderQuery query, CancellationToken ct)
    {
        var total = await db.BasicOrders.CountAsync(ct);

        var request = from o in db.BasicOrders
                      join c in db.BasicCustomers on o.CustomerId equals c.Id
                      join p in db.BasicPeople on c.PersonId equals p.Id
                      join e in db.BasicEmployees on o.EmployeeId equals e.Id
                      join pe in db.BasicPeople on e.Id equals pe.Id
                      select new GetAllOrderResponse(
                          o.Id,
                          o.UserId,
                          o.CustomerId,
                          p.Name,
                          o.TotalAmount,
                          o.SaleDate,
                          o.Status.ToString(),
                          o.Details.Count,
                          o.EmployeeId,
                          pe.Name
                      );

        var response = await request
            .OrderByDescending(o => o.SaleDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return new Pagination<List<GetAllOrderResponse>>(response,
            total,
            query.Page,
            query.PerPage);
    }
}
