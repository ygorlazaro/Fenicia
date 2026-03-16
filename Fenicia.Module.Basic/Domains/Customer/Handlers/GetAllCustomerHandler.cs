using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Customer.Queries;
using Fenicia.Module.Basic.Domains.Customer.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.Handlers;

/// <summary>
///     Handler responsible for retrieving all customers with pagination.
///     Returns a paginated list of customers including their associated person details.
/// </summary>
public class GetAllCustomerHandler(DefaultContext db)
{
    /// <summary>
    ///     Retrieves a paginated list of all customers.
    /// </summary>
    /// <param name="query">The query containing pagination parameters (page number and items per page).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated response containing the list of customers.</returns>
    public async Task<Pagination<List<GetAllCustomerResponse>>> Handle(GetAllCustomerQuery query, CancellationToken ct)
    {
        var total = await db.BasicCustomers.CountAsync(ct);

        var customers = await db.BasicCustomers
            .Include(c => c.Person)
            .Include(c => c.Person.PersonAddresses)
                .ThenInclude(pa => pa.Address)
                    .ThenInclude(a => a.State)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = customers.Select(c =>
        {
            var personAddress = c.Person.PersonAddresses.FirstOrDefault();
            var address = personAddress?.Address;
            
            return new GetAllCustomerResponse(
                c.Id, 
                c.PersonId, 
                c.Person.Name, 
                c.Person.Email, 
                c.Person.PhoneNumber, 
                c.Person.Document,
                address != null ? new AddressResponse(
                    address.Id,
                    address.Street,
                    address.Number,
                    address.Complement,
                    address.Neighborhood,
                    address.ZipCode,
                    address.StateId,
                    address.State?.Name,
                    address.City,
                    address.Country
                ) : null
            );
        }).ToList();

        return new Pagination<List<GetAllCustomerResponse>>(response, total, query.Page, query.PerPage);
    }
}