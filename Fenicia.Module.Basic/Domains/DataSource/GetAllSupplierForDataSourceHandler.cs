using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class GetAllSupplierForDataSourceHandler(DefaultContext context)
{
    public async Task<List<GetAllSupplierForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await context.BasicSuppliers
            .Include(s => s.Person)
            .OrderBy(s => s.Person.Name)
            .Select(s => new GetAllSupplierForDataSourceResponse(s.Id, s.Person.Name))
            .ToListAsync(ct);
    }
}

public record GetAllSupplierForDataSourceResponse(Guid Id, string Name);
