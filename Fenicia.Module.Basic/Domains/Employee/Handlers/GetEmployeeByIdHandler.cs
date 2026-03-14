using Fenicia.Common.Data.Contexts;
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
        var employee = await db.BasicEmployees.Include(e => e.Person).FirstOrDefaultAsync(e => e.Id == query.Id, ct);

        return employee switch
        {
            null => null,
            _ => new GetEmployeeByIdResponse(employee.Id, employee.PositionId, employee.PersonId, employee.Person.Name, employee.Person.Email, employee.Person.PhoneNumber, employee.Person.Document, employee.Person.Street, employee.Person.Number, employee.Person.Complement, employee.Person.Neighborhood, employee.Person.ZipCode, employee.Person.StateId, employee.Person.City)
        };

    }
}