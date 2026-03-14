using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all positions for datasource purposes.
///     Returns positions ordered alphabetically by name.
/// </summary>
public class GetAllPositionForDataSourceHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves all positions ordered by name.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of positions with ID and name.</returns>
    public async Task<List<GetAllPositionForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicPositions.OrderBy(p => p.Name).Select(p => new GetAllPositionForDataSourceResponse(p.Id, p.Name)).ToListAsync(ct);
    }
}