using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class GetAllEmployeeHandler(DefaultContext db) : IRequestHandler<GetAllEmployeeQuery, Pagination<List<GetAllEmployeeResponse>>>
{

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
