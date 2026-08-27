using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Responses;
using Fenicia.Module.Basic.Domains.Employee.Queries;
using Fenicia.Module.Basic.Domains.Employee.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Employee.Handlers;

public class GetEmployeesByPositionIdHandler(DefaultContext db) : IRequestHandler<GetEmployeesByPositionIdQuery, Pagination<List<GetEmployeesByPositionIdResponse>>>
{

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
