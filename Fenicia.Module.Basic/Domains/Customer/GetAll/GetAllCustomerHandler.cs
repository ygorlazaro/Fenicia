using Fenicia.Common;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetAll;

public class GetAllCustomerHandler(DefaultContext context)
{
    public async Task<Pagination<List<GetAllCustomerResponse>>> Handle(GetAllCustomerQuery query, CancellationToken ct)
    {
        var total = await context.BasicCustomers.CountAsync(ct);

        var customers = await context.BasicCustomers
            .Include(c => c.Person)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);

        var response = customers.Select(c => new GetAllCustomerResponse(
            c.Id,
            c.PersonId,
            c.Person.Name,
            c.Person.Email,
            c.Person.PhoneNumber,
            c.Person.Document,
            c.Person.Street,
            c.Person.Number,
            c.Person.Complement,
            c.Person.Neighborhood,
            c.Person.ZipCode,
            c.Person.StateId,
            c.Person.City)).ToList();

        return new Pagination<List<GetAllCustomerResponse>>(response, total, query.Page, query.PerPage);
    }
}
