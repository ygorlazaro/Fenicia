using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

public class ProductCategoryService(DefaultContext db)
{
    public async Task<Pagination<List<GetAllProductCategoryResponse>>> GetAllAsync(GetAllProductCategoryQuery query, CancellationToken ct)
    {
        var total = await db.BasicProductCategories.CountAsync(ct);

        var categories = await db.BasicProductCategories.Select(pc => new GetAllProductCategoryResponse(pc.Id, pc.Name)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);

        return new Pagination<List<GetAllProductCategoryResponse>>(categories, total, query.Page, query.PerPage);
    }

    public async Task<GetProductCategoryByIdResponse?> GetByIdAsync(GetProductCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == query.Id, ct);

        return category switch
        {
            null => null,
            _ => new GetProductCategoryByIdResponse(category.Id, category.Name)
        };
    }

    public async Task<AddProductCategoryResponse> AddAsync(AddProductCategoryCommand command, CancellationToken ct)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name
        };

        db.BasicProductCategories.Add(category);

        await db.SaveChangesAsync(ct);

        return new AddProductCategoryResponse(category.Id, category.Name);
    }

    public async Task<UpdateProductCategoryResponse?> UpdateAsync(UpdateProductCategoryCommand command, CancellationToken ct)
    {
        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (category is null)
        {
            return null;
        }

        category.Name = command.Name;

        db.BasicProductCategories.Update(category);

        await db.SaveChangesAsync(ct);

        return new UpdateProductCategoryResponse(category.Id, category.Name);
    }

    public async Task DeleteAsync(DeleteProductCategoryCommand command, CancellationToken ct)
    {
        var category = await db.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (category is null)
        {
            return;
        }

        category.Deleted = DateTime.Now;

        db.BasicProductCategories.Update(category);

        await db.SaveChangesAsync(ct);
    }
}
