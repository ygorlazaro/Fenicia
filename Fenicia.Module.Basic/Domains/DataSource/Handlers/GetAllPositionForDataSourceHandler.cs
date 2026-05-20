using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all positions for datasource purposes.
///     Returns positions ordered alphabetically by name.
/// </summary>
public class GetAllPositionForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllPositionForDataSourceQuery, List<GetAllPositionForDataSourceResponse>>
{
    /// <summary>
    ///     Retrieves all positions ordered by name.
    /// </summary>
    /// <param name="query">Datasource query marker.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of positions with ID and name.</returns>
    public async Task<List<GetAllPositionForDataSourceResponse>> Handle(GetAllPositionForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicPositions.OrderBy(p => p.Name).Select(p => new GetAllPositionForDataSourceResponse(p.Id, p.Name)).ToListAsync(ct);
    }
}
