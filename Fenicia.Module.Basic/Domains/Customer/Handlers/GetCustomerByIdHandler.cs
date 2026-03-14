using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Queries;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

/// <summary>
/// Handler responsible for retrieving a specific customer by their unique identifier.
/// Returns customer details including associated person information.
/// </summary>
public class GetCustomerByIdHandler(DefaultContext db)
{
    /// <summary>
    /// Retrieves a customer by their unique identifier.
    /// </summary>
    /// <param name="query">The query containing the customer ID to retrieve.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The customer details if found, null otherwise.</returns>
    public async Task<GetCustomerByIdResponse?> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        var customer = await db.BasicCustomers
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == query.Id,
                ct);

        return customer switch
        {
            null => null,
            _ => new GetCustomerByIdResponse(customer.Id,
                customer.PersonId,
                customer.Person.Name,
                customer.Person.Email,
                customer.Person.PhoneNumber,
                customer.Person.Document,
                customer.Person.Street,
                customer.Person.Number,
                customer.Person.Complement,
                customer.Person.Neighborhood,
                customer.Person.ZipCode,
                customer.Person.StateId,
                customer.Person.City)
        };
    }
}
