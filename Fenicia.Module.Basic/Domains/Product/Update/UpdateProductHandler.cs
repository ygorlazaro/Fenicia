using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Update;

public class UpdateProductHandler(DefaultContext context)
{
    public async Task<UpdateProductResponse?> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await context.BasicProducts
            .FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product is null)
        {
            return null;
        }

        product.Name = command.Name;
        product.CostPrice = command.CostPrice;
        product.SalesPrice = command.SalesPrice;
        product.Quantity = command.Quantity;
        product.CategoryId = command.CategoryId;
        product.SupplierId = command.SupplierId;

        context.BasicProducts.Update(product);

        await context.SaveChangesAsync(ct);

        var category = await context.BasicProductCategories
            .FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        BasicSupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await context.BasicSuppliers
                .Include(s => s.Person)
                .FirstOrDefaultAsync(s => s.Id == product.SupplierId, ct);
        }

        return new UpdateProductResponse(
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
