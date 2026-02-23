using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Customer.Add;

public class AddCustomerHandler(BasicContext context)
{
    public async Task<CustomerResponse> Handle(AddCustomerCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Cpf = command.Cpf,
            PhoneNumber = command.PhoneNumber ?? string.Empty,
            Street = command.Street ?? string.Empty,
            Number = command.Number ?? string.Empty,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode ?? string.Empty,
            StateId = command.StateId,
            City = command.City ?? string.Empty
        };

        var customer = new CustomerModel
        {
            Id = command.Id,
            Person = person,
            PersonId = person.Id
        };

        context.Customers.Add(customer);

        await context.SaveChangesAsync(ct);

        return new CustomerResponse(
            customer.Id,
            new PersonResponse(
                person.Name,
                person.Email,
                person.Cpf,
                person.PhoneNumber,
                new AddressResponse(
                    person.City,
                    person.Complement,
                    person.Neighborhood,
                    person.Number,
                    person.StateId,
                    person.Street,
                    person.ZipCode)));
    }
}
