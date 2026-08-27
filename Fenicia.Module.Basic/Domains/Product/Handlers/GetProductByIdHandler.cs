using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.DTOs.Queries;
using Fenicia.Module.Basic.Domains.Product.DTOs.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class GetProductByIdHandler(DefaultContext db) : IRequestHandler<GetProductByIdQuery, GetProductByIdResponse?>
{
    public async Task<GetProductByIdResponse?> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        if (product is null)
        {
            return null;
        }

        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await db.BasicSuppliers.Include(s => s.Person).FirstOrDefaultAsync(s => s.Id == product.SupplierId, ct);
        }

        return new GetProductByIdResponse(
            product.Id,
            product.Name,
            product.SKU,
            product.Barcode,
            product.Description,
            product.CostPrice,
            product.SalesPrice,
            product.Quantity,
            product.MinStockLevel,
            product.MaxStockLevel,
            product.ImageUrl,
            product.Weight,
            product.Dimensions,
            product.UnitOfMeasure,
            product.CategoryId,
            category?.Name ?? string.Empty,
            product.SupplierId,
            supplier?.Person?.Name,
            product.IsActive);
    }
}