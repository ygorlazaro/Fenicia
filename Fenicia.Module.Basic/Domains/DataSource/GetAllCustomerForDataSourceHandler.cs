using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllCustomerForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllCustomerForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicCustomers
            .AsNoTracking()
            .OrderBy(c => c.Person.Name)
            .Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.Person.Name))
            .ToListAsync(ct);
    }
}