using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

/// <summary>
///     Handler responsible for retrieving employees filtered by position ID.
///     Returns a paginated list of employees with the specified position.
/// </summary>
public class GetEmployeesByPositionIdHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves employees filtered by position ID with pagination.
    /// </summary>
    /// <param name="query">The query containing position ID and pagination parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of employees with the specified position.</returns>
    public async Task<List<GetEmployeesByPositionIdResponse>> Handle(GetEmployeesByPositionIdQuery query, CancellationToken ct)
    {
        return await db.BasicEmployees.Where(e => e.PositionId == query.PositionId).Select(e => new GetEmployeesByPositionIdResponse(e.Id, e.PositionId, e.PersonId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

    }
}