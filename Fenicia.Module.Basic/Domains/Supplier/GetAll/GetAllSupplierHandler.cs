using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetAll;

public class GetAllSupplierHandler(DefaultContext context)
{
    public async Task<List<GetAllSupplierResponse>> Handle(GetAllSupplierQuery query, CancellationToken ct)
    {
        return await context.BasicSuppliers
             .Select(s => new GetAllSupplierResponse(s.Id, s.Cnpj))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
