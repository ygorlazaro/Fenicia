using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Product.Add;

public class AddProductHandler(BasicContext context)
{
    public async Task<AddProductResponse> Handle(AddProductCommand command, CancellationToken ct)
    {
        var product = new ProductModel
        {
            Id = command.Id,
            Name = command.Name,
            CostPrice = command.CostPrice,
            SalesPrice = command.SellingPrice,
            Quantity = command.Quantity,
            CategoryId = command.CategoryId
        };

        context.Products.Add(product);

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
