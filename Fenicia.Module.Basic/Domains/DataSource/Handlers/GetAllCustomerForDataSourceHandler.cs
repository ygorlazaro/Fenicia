using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllCustomerForDataSourceHandler(DefaultContext db)
{
    public async Task<List<GetAllCustomerForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicCustomers
            .AsNoTracking()
            .OrderBy(c => c.Person.Name)
            .Select(c => new GetAllCustomerForDataSourceResponse(c.Id,
                c.Person.Name))
            .ToListAsync(ct);
    }
}