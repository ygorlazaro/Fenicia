using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class GetEmployeeByIdHandler(DefaultContext db) : IRequestHandler<GetEmployeeByIdQuery, GetEmployeeByIdResponse?>
{

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
