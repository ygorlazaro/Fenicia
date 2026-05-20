using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all suppliers for datasource purposes.
///     Returns suppliers ordered alphabetically by person name.
/// </summary>
public class GetAllSupplierForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllSupplierForDataSourceQuery, List<GetAllSupplierForDataSourceResponse>>
{
    /// <summary>
    ///     Retrieves all suppliers ordered by person name.
    /// </summary>
    /// <param name="query">Datasource query marker.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of suppliers with ID and name.</returns>
    public async Task<List<GetAllSupplierForDataSourceResponse>> Handle(GetAllSupplierForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicSuppliers.OrderBy(s => s.Person.Name).Select(s => new GetAllSupplierForDataSourceResponse(s.Id, s.Person.Name)).ToListAsync(ct);
    }
}
