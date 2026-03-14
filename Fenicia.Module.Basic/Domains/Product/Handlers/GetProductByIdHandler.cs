using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.Queries;
using Fenicia.Module.Basic.Domains.Product.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

/// <summary>
/// Handler responsible for retrieving a specific product by its ID.
/// Returns product details including category and supplier information.
/// </summary>
public class GetProductByIdHandler(DefaultContext db)
{
    /// <summary>
    /// Retrieves a product by its ID.
    /// </summary>
    /// <param name="query">The query containing the product ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The product details if found, otherwise null.</returns>
    public async Task<GetProductByIdResponse?> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await db.BasicProducts
            .FirstOrDefaultAsync(p => p.Id == query.Id,
                ct);

        if (product is null)
        {
            return null;
        }

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
