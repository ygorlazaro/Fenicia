using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetAll;

public class GetAllCustomerHandler(BasicContext context)
{
    public async Task<List<GetAllCustomerResponse>> Handle(GetAllCustomerQuery query, CancellationToken ct)
    {
        var customers = await context.Customers
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .Select(c => new GetAllCustomerResponse(c.Id, c.PersonId))
            .ToListAsync(ct);

        return customers;
    }
}
