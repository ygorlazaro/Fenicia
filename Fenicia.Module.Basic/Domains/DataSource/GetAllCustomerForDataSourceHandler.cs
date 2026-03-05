using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllCustomerForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllCustomerForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicCustomers
            .AsNoTracking()
            .OrderBy(c => c.PersonModel.Name)
            .Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.PersonModel.Name))
            .ToListAsync(ct);
    }
}

public record GetAllCustomerForDataSourceResponse(Guid Id, string Name);
