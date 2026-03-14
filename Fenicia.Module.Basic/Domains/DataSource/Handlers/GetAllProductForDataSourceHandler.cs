using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all products for datasource purposes.
///     Returns products ordered alphabetically by name.
/// </summary>
public class GetAllProductForDataSourceHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves all products ordered by name.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of products with ID and name.</returns>
    public async Task<List<GetAllProductForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicProducts.OrderBy(p => p.Name).Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name)).ToListAsync(ct);
    }
}