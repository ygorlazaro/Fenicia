using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

namespace Fenicia.Module.Basic.Domains.Product.Add;

public class AddProductHandler(DefaultContext context)
{
    public async Task<AddProductResponse> Handle(AddProductCommand command, CancellationToken ct)
    {
        var product = new BasicProductModel
        {
            Id = command.Id,
            Name = command.Name,
            CostPrice = command.CostPrice,
            SalesPrice = command.SellingPrice,
            Quantity = command.Quantity,
            CategoryId = command.CategoryId
        };

        context.BasicProducts.Add(product);

        await context.SaveChangesAsync(ct);

        return new AddProductResponse(
            product.Id,
            product.Name,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.CategoryId,
            string.Empty);
    }
}
