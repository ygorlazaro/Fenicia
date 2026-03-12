using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

public class UpdateCustomerHandler(DefaultContext db)
{
    public async Task<UpdateCustomerResponse?> Handle(UpdateCustomerCommand command, CancellationToken ct)
    {
        var customer = await db.BasicCustomers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == command.Id,
                ct);

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

        db.BasicCustomers.Update(customer);

        await db.SaveChangesAsync(ct);

        return new UpdateCustomerResponse(customer.Id,
            customer.PersonId);
    }
}
