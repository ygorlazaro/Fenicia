using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

/// <summary>
///     Handler responsible for retrieving all employees with pagination.
///     Returns a paginated list of employees including their associated person and position details.
/// </summary>
public class GetAllEmployeeHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves a paginated list of all employees.
    /// </summary>
    /// <param name="query">The query containing pagination parameters (page number and items per page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing the list of employees.</returns>
    public async Task<Pagination<List<GetAllEmployeeResponse>>> Handle(GetAllEmployeeQuery query, CancellationToken ct)
    {
        var total = await db.BasicEmployees.CountAsync(ct);

        var employees = await db.BasicEmployees
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

            return new GetAllEmployeeResponse(
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

        return new Pagination<List<GetAllEmployeeResponse>>(response, total, query.Page, query.PerPage);
    }
}
