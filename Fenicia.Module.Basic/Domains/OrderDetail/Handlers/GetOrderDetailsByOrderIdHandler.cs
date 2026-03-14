using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.OrderDetail.Queries;
using Fenicia.Module.Basic.Domains.OrderDetail.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.OrderDetail.Handlers;

/// <summary>
///     Handler for retrieving order details by order ID.
/// </summary>
/// <remarks>
///     This handler:
///     1. Queries the database for order details matching the given OrderId
///     2. Projects the data into GetOrderDetailsByOrderIdResponse
///     3. Returns a list of all matching order detail items
/// </remarks>
public class GetOrderDetailsByOrderIdHandler(DefaultContext db)
{
    /// <summary>
    ///     Handles the retrieval of order details by order ID.
    /// </summary>
    /// <param name="query">The query containing the order ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of order detail responses.</returns>
    public async Task<List<GetOrderDetailsByOrderIdResponse>> Handle(GetOrderDetailsByOrderIdQuery query, CancellationToken ct)
    {
        var details = await db.BasicOrderDetails.Where(d => d.OrderId == query.OrderId).Select(d => new GetOrderDetailsByOrderIdResponse(d.Id, d.OrderId, d.ProductId, d.Price, d.Quantity)).ToListAsync(ct);

        return details;
    }
}