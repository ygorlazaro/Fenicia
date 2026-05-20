using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Handlers;

public class UpdateProductHandler(DefaultContext db) : IRequestHandler<UpdateProductCommand, UpdateProductResponse?>
{
    public async Task<UpdateProductResponse?> Handle(UpdateProductCommand command, CancellationToken ct)
    {
        var product = await db.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product is null)
        {
            return null;
        }

        product.Name = command.Name;
        product.SKU = command.SKU;
        product.Barcode = command.Barcode;
        product.Description = command.Description;
        product.CostPrice = command.CostPrice;
        product.SalesPrice = command.SalesPrice;
        product.Quantity = command.Quantity;
        product.MinStockLevel = command.MinStockLevel;
        product.MaxStockLevel = command.MaxStockLevel;
        product.ImageUrl = command.ImageUrl;
        product.Weight = command.Weight;
        product.Dimensions = command.Dimensions;
        product.UnitOfMeasure = command.UnitOfMeasure;
        product.CategoryId = command.CategoryId;
        product.SupplierId = command.SupplierId;

        db.BasicProducts.Update(product);

        await db.SaveChangesAsync(ct);

        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == product.CategoryId, ct);

        SupplierModel? supplier = null;
        if (product.SupplierId.HasValue)
        {
            supplier = await db.BasicSuppliers.Include(s => s.Person).FirstOrDefaultAsync(s => s.Id == product.SupplierId, ct);
        }

        return new UpdateProductResponse(
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
            supplier?.Person.Name,
            product.IsActive);
    }
}