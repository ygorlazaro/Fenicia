using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Queries;
using Fenicia.Module.Basic.Domains.Position.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

/// <summary>
///     Handler for retrieving a position by its ID.
/// </summary>
/// <remarks>
///     This handler:
///     1. Queries the database for a position with the specified ID
///     2. Returns null if position is not found
///     3. Returns the position data if found
/// </remarks>
public class GetPositionByIdHandler(DefaultContext db)
{
    /// <summary>
    ///     Handles retrieval of a position by ID.
    /// </summary>
    /// <param name="query">The query containing the position ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The position response if found, null otherwise.</returns>
    public async Task<GetPositionByIdResponse?> Handle(GetPositionByIdQuery query, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        return position is null ? null : new GetPositionByIdResponse(position.Id, position.Name);
    }
}