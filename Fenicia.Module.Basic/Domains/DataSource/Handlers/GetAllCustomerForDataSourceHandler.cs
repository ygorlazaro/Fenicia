using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Queries;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

/// <summary>
///     Handler responsible for retrieving all customers for datasource purposes.
///     Returns customers ordered alphabetically by person name. Uses AsNoTracking for read-only queries.
/// </summary>
public class GetAllCustomerForDataSourceHandler(DefaultContext db) : IRequestHandler<GetAllCustomerForDataSourceQuery, List<GetAllCustomerForDataSourceResponse>>
{
    /// <summary>
    ///     Retrieves all customers ordered by person name.
    /// </summary>
    /// <param name="query">Datasource query marker.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of customers with ID and name.</returns>
    public async Task<List<GetAllCustomerForDataSourceResponse>> Handle(GetAllCustomerForDataSourceQuery query, CancellationToken ct)
    {
        return await db.BasicCustomers.AsNoTracking().OrderBy(c => c.Person.Name).Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.Person.Name)).ToListAsync(ct);
    }
}
