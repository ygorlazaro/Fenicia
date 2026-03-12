using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Responses;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class AddProductHandler(DefaultContext db)
{
    public async Task<AddProductResponse> Handle(AddProductCommand command, CancellationToken ct)
    {
        var product = new ProductModel
        {
            Id = command.Id,
            Name = command.Name,
            CostPrice = command.CostPrice,
            SalesPrice = command.SalesPrice,
            Quantity = command.Quantity,
            CategoryId = command.CategoryId,
            SupplierId = command.SupplierId
        };

        db.BasicProducts.Add(product);

        await db.SaveChangesAsync(ct);

        return new AddProductResponse(
            product.Id,
            product.Name,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.CategoryId,
            string.Empty,
            product.SupplierId,
            string.Empty);
    }
}
