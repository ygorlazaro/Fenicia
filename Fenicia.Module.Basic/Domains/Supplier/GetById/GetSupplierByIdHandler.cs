using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Supplier.GetById;

public class GetSupplierByIdHandler(BasicContext context)
{
    public async Task<GetSupplierByIdResponse?> Handle(GetSupplierByIdQuery query, CancellationToken ct)
    {
        return await context.Suppliers
            .Select(s => new GetSupplierByIdResponse(s.Id, s.Cnpj))
            .FirstOrDefaultAsync(s => s.Id == query.Id, ct);
    }
}