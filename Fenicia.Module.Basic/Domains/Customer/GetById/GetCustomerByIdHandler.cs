using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Customer.GetById;

public class GetCustomerByIdHandler(DefaultContext context)
{
    public async Task<GetCustomerByIdResponse?> Handle(GetCustomerByIdQuery query, CancellationToken ct)
    {
        return await context.BasicCustomers
            .Select(c => new GetCustomerByIdResponse(c.Id, c.PersonId))
            .FirstOrDefaultAsync(c => c.Id == query.Id, ct);
    }
}
