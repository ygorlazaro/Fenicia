using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Update;

public class UpdateCustomerHandler(DefaultContext context)
{
    public async Task<UpdateCustomerResponse?> Handle(UpdateCustomerCommand command, CancellationToken ct)
    {
        var customer = await context.BasicCustomers
            .Include(c => c.PersonModel)
            .FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (customer is null)
        {
            return null;
        }

        customer.PersonModel.Name = command.Name;
        customer.PersonModel.Email = command.Email;
        customer.PersonModel.Document = command.Document;
        customer.PersonModel.PhoneNumber = command.PhoneNumber;
        customer.PersonModel.Street = command.Street;
        customer.PersonModel.Number = command.Number;
        customer.PersonModel.Complement = command.Complement;
        customer.PersonModel.Neighborhood = command.Neighborhood;
        customer.PersonModel.ZipCode = command.ZipCode;
        customer.PersonModel.StateId = command.StateId;
        customer.PersonModel.City = command.City;

        context.BasicCustomers.Update(customer);

        await context.SaveChangesAsync(ct);

        return new UpdateCustomerResponse(customer.Id, customer.PersonId);
    }
}
