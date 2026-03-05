using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetById;

public class GetProductByIdHandler(DefaultContext context)
{
    public async Task<GetProductByIdResponse?> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await context.BasicProducts
            .FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        if (product is null)
        {
            return null;
        }

        var category = await context.BasicProductCategories
            .FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        BasicSupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await context.BasicSuppliers
                .Include(s => s.Person)
                .FirstOrDefaultAsync(s => s.Id == product.SupplierId, ct);
        }

        return new GetProductByIdResponse(
            product.Id,
            product.Name,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.CategoryId,
            category?.Name ?? string.Empty,
            product.SupplierId,
            supplier?.Person?.Name);
    }
}
