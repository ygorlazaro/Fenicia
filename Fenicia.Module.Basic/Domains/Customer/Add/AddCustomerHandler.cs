using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Customer.Add;

public class AddCustomerHandler(BasicContext context)
{
    public async Task<AddCustomerResponse> Handle(AddCustomerCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber,
            Street = command.Street,
            Number = command.Number,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode,
            StateId = command.StateId,
            City = command.City
        };

        var customer = new CustomerModel
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id
        };

        context.Customers.Add(customer);

        await context.SaveChangesAsync(ct);

        return new AddCustomerResponse(customer.Id, person.Id);
    }
}
