using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all suppliers for datasource purposes.
///     Returns suppliers ordered alphabetically by person name.
/// </summary>
public class GetAllSupplierForDataSourceHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves all suppliers ordered by person name.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of suppliers with ID and name.</returns>
    public async Task<List<GetAllSupplierForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicSuppliers.OrderBy(s => s.Person.Name).Select(s => new GetAllSupplierForDataSourceResponse(s.Id, s.Person.Name)).ToListAsync(ct);
    }
}