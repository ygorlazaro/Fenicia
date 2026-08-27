using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Order.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Order.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Order.Handlers;

public class GetAllOrderHandler(DefaultContext db) : IRequestHandler<GetAllOrderQuery, Pagination<List<GetAllOrderResponse>>>
{
    public async Task<Pagination<List<GetAllOrderResponse>>> Handle(GetAllOrderQuery query, CancellationToken ct)
    {
        var total = await db.BasicOrders.CountAsync(ct);

        var orderIds = await db.BasicOrders
            .OrderByDescending(o => o.SaleDate)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(o => o.Id)
            .ToListAsync(ct);

        var detailCounts = await db.BasicOrderDetails
            .Where(d => orderIds.Contains(d.OrderId))
            .GroupBy(d => d.OrderId)
            .Select(g => new { OrderId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.OrderId, g => g.Count, ct);

        var orders = await (from o in db.BasicOrders
                            where orderIds.Contains(o.Id)
                            select new
                            {
                                o.Id,
                                o.OrderNumber,
                                o.UserId,
                                o.CustomerId,
                                CustomerName = o.Customer.Person.Name,
                                o.TotalAmount,
                                o.DiscountAmount,
                                o.TotalQuantity,
                                o.SaleDate,
                                o.Status,
                                o.PaymentMethod,
                                o.EmployeeId,
                                EmployeeName = o.Employee != null ? o.Employee.Person.Name : null
                            }).ToListAsync(ct);

        var response = orders
            .OrderByDescending(o => o.SaleDate)
            .Select(o => new GetAllOrderResponse(
                o.Id,
                o.OrderNumber,
                o.UserId,
                o.CustomerId,
                o.CustomerName,
                o.TotalAmount,
                o.DiscountAmount,
                o.TotalQuantity,
                o.SaleDate,
                o.Status.ToString(),
                o.PaymentMethod,
                detailCounts.TryGetValue(o.Id, out var count) ? count : 0,
                o.EmployeeId,
                o.EmployeeName))
            .ToList();

        return new Pagination<List<GetAllOrderResponse>>(response, total, query.Page, query.PerPage);
    }
}
