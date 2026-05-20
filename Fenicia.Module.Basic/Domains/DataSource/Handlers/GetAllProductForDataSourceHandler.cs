using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all products for datasource purposes.
///     Returns products ordered alphabetically by name.
/// </summary>
public class GetAllProductForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllProductForDataSourceQuery, List<GetAllProductForDataSourceResponse>>
{
    /// <summary>
    ///     Retrieves all products ordered by name.
    /// </summary>
    /// <param name="query">Datasource query marker.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of products with ID and name.</returns>
    public async Task<List<GetAllProductForDataSourceResponse>> Handle(GetAllProductForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicProducts.OrderBy(p => p.Name).Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name)).ToListAsync(ct);
    }
}
