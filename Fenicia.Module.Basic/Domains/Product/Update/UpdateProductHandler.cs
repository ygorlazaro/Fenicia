using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Update;

public class UpdateProductHandler(BasicContext context)
{
    public async Task<ProductResponse?> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product is null) return null;

        product.Name = command.Name;
        product.CostPrice = command.CostPrice;
        product.SalesPrice = command.SellingPrice;
        product.Quantity = command.Quantity;
        product.CategoryId = command.CategoryId;

        context.Products.Update(product);

        await context.SaveChangesAsync(ct);

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
