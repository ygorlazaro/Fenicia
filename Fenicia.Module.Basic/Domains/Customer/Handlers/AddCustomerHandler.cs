using Fenicia.Common.Data.Contexts;
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
            PhoneNumber = command.PhoneNumber,
            Street = command.Street,
            Number = command.Number,
            Complement = command.Complement,
            Neighborhood = command.Neighborhood,
            ZipCode = command.ZipCode,
            StateId = command.StateId,
            City = command.City
        };

        var customer = new CustomerModel { Id = command.Id, Person = person, PersonId = person.Id };

        db.BasicCustomers.Add(customer);

        await db.SaveChangesAsync(ct);

        return new AddCustomerResponse(customer.Id, person.Id);
    }
}