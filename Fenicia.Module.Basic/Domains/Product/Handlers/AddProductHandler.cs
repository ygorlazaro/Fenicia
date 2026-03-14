using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Responses;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

/// <summary>
/// Handler responsible for creating a new product.
/// Adds a new product to the database with the provided details.
/// </summary>
public class AddProductHandler(DefaultContext db)
{
    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">The command containing product details.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created product with its details.</returns>
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
