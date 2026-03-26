using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

/// <summary>
///     Handler responsible for retrieving a specific employee by their unique identifier.
///     Returns employee details including associated person information.
/// </summary>
public class GetEmployeeByIdHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves an employee by their unique identifier.
    /// </summary>
    /// <param name="query">The query containing the employee ID to retrieve.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The employee details if found, null otherwise.</returns>
    public async Task<GetEmployeeByIdResponse?> Handle(GetEmployeeByIdQuery query, CancellationToken ct)
    {
        var employee = await db.BasicEmployees
            .Include(e => e.Person)
            .Include(e => e.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(e => e.Id == query.Id, ct);

        if (employee == null)
            return null;

        var personAddress = employee.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetEmployeeByIdResponse(
            employee.Id, 
            employee.PositionId, 
            employee.PersonId, 
            employee.Person.Name, 
            employee.Person.Email, 
            employee.Person.PhoneNumber, 
            employee.Person.Document,
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
    }
}