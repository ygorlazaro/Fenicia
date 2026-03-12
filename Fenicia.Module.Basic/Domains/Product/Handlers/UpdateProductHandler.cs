using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class UpdateProductHandler(DefaultContext db)
{
    public async Task<UpdateProductResponse?> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await db.BasicProducts
            .FirstOrDefaultAsync(p => p.Id == command.Id,
                ct);

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

        db.BasicProducts.Update(product);

        await db.SaveChangesAsync(ct);

        var category = await db.BasicProductCategories
            .FirstOrDefaultAsync(c => c.Id == product.CategoryId,
                ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await db.BasicSuppliers
                .Include(s => s.Person)
                .FirstOrDefaultAsync(s => s.Id == product.SupplierId,
                    ct);
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
            supplier?.Person.Name);
    }
}
