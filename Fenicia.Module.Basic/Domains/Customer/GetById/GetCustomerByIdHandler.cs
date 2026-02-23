using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetById;

public class GetCustomerByIdHandler(BasicContext context)
{
    public async Task<CustomerResponse?> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await context.Customers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        if (customer is null) return null;

        return new CustomerResponse(
            customer.Id,
            new PersonResponse(
                customer.Person.Name,
                customer.Person.Email,
                customer.Person.Document,
                customer.Person.PhoneNumber,
                new AddressResponse(
                    customer.Person.City,
                    customer.Person.Complement,
                    customer.Person.Neighborhood,
                    customer.Person.Number,
                    customer.Person.StateId,
                    customer.Person.Street,
                    customer.Person.ZipCode)));
    }
}
