using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetAll;

public class GetAllCustomerHandler(BasicContext context)
{
    public async Task<List<CustomerResponse>> Handle(GetAllCustomerQuery query, CancellationToken ct)
    {
        var customers = await context.Customers
            .Include(c => c.Person)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        return customers.Select(c => new CustomerResponse(
            c.Id,
            new PersonResponse(
                c.Person.Name,
                c.Person.Email,
                c.Person.Document,
                c.Person.PhoneNumber,
                new AddressResponse(
                    c.Person.City,
                    c.Person.Complement,
                    c.Person.Neighborhood,
                    c.Person.Number,
                    c.Person.StateId,
                    c.Person.Street,
                    c.Person.ZipCode)))).ToList();
    }
}
