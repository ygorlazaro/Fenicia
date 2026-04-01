using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Responses;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

/// <summary>
///     Handler responsible for creating new customers in the system.
///     Creates both the customer record and associated person record.
/// </summary>
public class AddCustomerHandler(DefaultContext db)
{
    /// <summary>
    ///     Creates a new customer with the provided command data.
    /// </summary>
    /// <param name="command">The customer creation command containing all customer details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created customer response with customer and person IDs.</returns>
    public async Task<AddCustomerResponse> Handle(AddCustomerCommand command, CancellationToken ct)
    {
        var person = new PersonModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Email = command.Email,
            Document = command.Document,
            PhoneNumber = command.PhoneNumber
        };

        AddressModel? address = null;
        
        if (command.Address != null)
        {
            address = new AddressModel
            {
                Id = Guid.NewGuid(),
                Street = command.Address.Street,
                Number = command.Address.Number,
                Complement = command.Address.Complement,
                Neighborhood = command.Address.Neighborhood,
                ZipCode = command.Address.ZipCode,
                StateId = command.Address.StateId,
                City = command.Address.City,
                Country = command.Address.Country
            };
            db.AuthAddresses.Add(address);
        }

        var customer = new CustomerModel
        {
            Person = person,
            PersonId = person.Id
        };

        if (address != null)
        {
            var personAddress = new PersonAddressModel
            {
                Id = Guid.NewGuid(),
                PersonId = person.Id,
                AddressId = address.Id
            };
            db.BasicPersonAddresses.Add(personAddress);
        }

        db.BasicCustomers.Add(customer);

        await db.SaveChangesAsync(ct);

        return new AddCustomerResponse(customer.Id, person.Id);
    }
}
