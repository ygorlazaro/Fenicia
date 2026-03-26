using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
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
    public async Task<Pagination<List<GetEmployeesByPositionIdResponse>>> Handle(GetEmployeesByPositionIdQuery query, CancellationToken ct)
    {
        var total = await db.BasicEmployees.CountAsync(e => e.PositionId == query.PositionId, ct);

        var employees = await db.BasicEmployees
            .Where(e => e.PositionId == query.PositionId)
            .Include(e => e.Person)
            .Include(e => e.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Include(e => e.Position)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = employees.Select(e =>
        {
            var personAddress = e.Person.PersonAddresses.FirstOrDefault();
            var address = personAddress?.Address;

            return new GetEmployeesByPositionIdResponse(
                e.Id,
                e.PositionId,
                e.PersonId,
                e.Person.Name,
                e.Person.Email,
                e.Person.PhoneNumber,
                e.Person.Document,
                e.Position.Name,
                address != null ? new AddressResponse(
                    address.Id,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.Neighborhood,
                    address.ZipCode,
                    address.StateId,
                    address.State?.Name,
                    address.City,
                    address.Country
                ) : null
            );
        }).ToList();

        return new Pagination<List<GetEmployeesByPositionIdResponse>>(response, total, query.Page, query.PerPage);
    }
}