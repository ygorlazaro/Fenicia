using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

public class UpdateProductCategoryHandler(DefaultContext db) : IRequestHandler<UpdateProductCategoryCommand, UpdateProductCategoryResponse?>
{

    public async Task<UpdateProductCategoryResponse?> Handle(UpdateProductCategoryCommand command, CancellationToken ct)
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
}