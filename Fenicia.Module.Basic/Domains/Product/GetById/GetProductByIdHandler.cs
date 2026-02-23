using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.GetAll;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetById;

public class GetProductByIdHandler(BasicContext context)
{
    public async Task<ProductResponse?> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        if (product is null) return null;

        return new ProductResponse(
            product.Id,
            product.Name,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.CategoryId,
            product.Category.Name);
    }
}
