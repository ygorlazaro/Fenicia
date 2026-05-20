using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all employees for datasource purposes.
///     Returns employees ordered alphabetically by person name. Uses AsNoTracking for read-only queries.
/// </summary>
public class GetAllEmployeeForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllEmployeeForDataSourceQuery, List<GetAllEmployeeForDataSourceResponse>>
{
    /// <summary>
    ///     Retrieves all employees ordered by person name.
    /// </summary>
    /// <param name="query">Datasource query marker.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of employees with ID and name.</returns>
    public async Task<List<GetAllEmployeeForDataSourceResponse>> Handle(GetAllEmployeeForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicEmployees.AsNoTracking().OrderBy(e => e.Person.Name).Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name)).ToListAsync(ct);
    }
}
