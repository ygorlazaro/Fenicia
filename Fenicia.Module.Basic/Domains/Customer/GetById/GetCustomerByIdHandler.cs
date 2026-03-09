using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetById;

public class GetCustomerByIdHandler(DefaultContext context)
{
    public async Task<GetCustomerByIdResponse?> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await context.BasicCustomers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        if (customer is null)
            return null;

        return new GetCustomerByIdResponse(
            customer.Id,
            customer.PersonId,
            customer.Person.Name,
            customer.Person.Email,
            customer.Person.PhoneNumber,
            customer.Person.Document,
            customer.Person.Street,
            customer.Person.Number,
            customer.Person.Complement,
            customer.Person.Neighborhood,
            customer.Person.ZipCode,
            customer.Person.StateId,
            customer.Person.City);
    }
}
