using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Queries;
using Fenicia.Module.Basic.Domains.Position.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

/// <summary>
///     Handler for retrieving all positions with pagination.
/// </summary>
/// <remarks>
///     This handler:
///     1. Counts total positions in the database
///     2. Selects positions with Id and Name projection
///     3. Applies pagination using Skip and Take
///     4. Returns paginated result with total count
/// </remarks>
public class GetAllPositionHandler(DefaultContext db)
{
    /// <summary>
    ///     Handles retrieval of all positions with pagination support.
    /// </summary>
    /// <param name="query">The pagination query.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated list of positions.</returns>
    public async Task<Pagination<List<GetAllPositionResponse>>> Handle(GetAllPositionQuery query, CancellationToken ct)
    {
        var total = await db.BasicPositions.CountAsync(ct);

        var positions = await db.BasicPositions.Select(p => new GetAllPositionResponse(p.Id, p.Name)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        return new Pagination<List<GetAllPositionResponse>>(positions, total, query.Page, query.PerPage);
    }
}