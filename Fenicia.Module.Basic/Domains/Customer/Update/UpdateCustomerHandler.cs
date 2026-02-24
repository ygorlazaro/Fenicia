using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Update;

public class UpdateCustomerHandler(BasicContext context)
{
    public async Task<UpdateCustomerResponse?> Handle(UpdateCustomerCommand command, CancellationToken ct)
    {
        var customer = await context.Customers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (customer is null)
        {
            return null;
        }

        customer.Person.Name = command.Name;
        customer.Person.Email = command.Email;
        customer.Person.Document = command.Document;
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

        return new UpdateCustomerResponse(customer.Id, customer.PersonId);
    }
}