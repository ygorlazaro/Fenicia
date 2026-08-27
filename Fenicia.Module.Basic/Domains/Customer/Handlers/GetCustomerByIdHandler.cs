using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Queries;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

public class GetCustomerByIdHandler(DefaultContext db) : IRequestHandler<GetCustomerByIdQuery, GetCustomerByIdResponse?>
{

    public async Task<GetCustomerByIdResponse?> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await db.BasicCustomers
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        if (customer == null)
            return null;

        var personAddress = customer.Person.PersonAddresses.FirstOrDefault();
        var address = personAddress?.Address;

        return new GetCustomerByIdResponse(
            customer.Id,
            customer.PersonId,
            customer.Person.Name,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            customer.Person.Document,
            address != null ? new AddressResponse(
                address.Id,
                address.Street,
                address.Number,
                address.Complement,
                address.Neighborhood,
                address.ZipCode,
                address.StateId,
                address.State.Name,
                address.City,
                address.Country
            ) : null
        );
    }
}
