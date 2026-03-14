using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Commands;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

/// <summary>
///     Handler responsible for updating existing customer information.
///     Updates both the customer and associated person records.
/// </summary>
public class UpdateCustomerHandler(DefaultContext db)
{
    /// <summary>
    ///     Updates an existing customer's information.
    /// </summary>
    /// <param name="command">The update command containing the new customer data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated customer response if found, null if customer does not exist.</returns>
    public async Task<UpdateCustomerResponse?> Handle(UpdateCustomerCommand command, CancellationToken ct)
    {
        var customer = await db.BasicCustomers.Include(c => c.Person).FirstOrDefaultAsync(c => c.Id == command.Id, ct);

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

        return new UpdateCustomerResponse(customer.Id, customer.PersonId);
    }
}