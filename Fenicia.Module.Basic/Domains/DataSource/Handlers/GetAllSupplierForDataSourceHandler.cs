using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource.Handlers;

public class GetAllSupplierForDataSourceHandler(DefaultContext db)
{
    public async Task<List<GetAllSupplierForDataSourceResponse>> Handle(CancellationToken ct)
    {
        return await db.BasicSuppliers
            .OrderBy(s => s.Person.Name)
            .Select(s => new GetAllSupplierForDataSourceResponse(s.Id,
                s.Person.Name))
            .ToListAsync(ct);
    }
}