using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Update;

public class UpdateCustomerHandler(BasicContext context)
{
    public async Task<CustomerResponse?> Handle(UpdateCustomerCommand command, CancellationToken ct)
    {
        var customer = await context.Customers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (customer is null) return null;

        customer.Person.Name = command.Name;
        customer.Person.Email = command.Email;
        customer.Person.Cpf = command.Cpf;
        customer.Person.Document = command.Cpf;
        customer.Person.PhoneNumber = command.PhoneNumber;
        customer.Person.Street = command.Street;
        customer.Person.Number = command.Number;
        customer.Person.Complement = command.Complement;
        customer.Person.Neighborhood = command.Neighborhood;
        customer.Person.ZipCode = command.ZipCode;
        customer.Person.StateId = command.StateId;
        customer.Person.City = command.City;

        context.Customers.Update(customer);

        await context.SaveChangesAsync(ct);

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
