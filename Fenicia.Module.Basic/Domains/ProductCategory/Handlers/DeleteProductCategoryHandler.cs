using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

public class DeleteProductCategoryHandler(DefaultContext context) : IRequestHandler<DeleteProductCategoryCommand>
{

    public async Task Handle(DeleteProductCategoryCommand command, CancellationToken ct)
    {
        var category = await context.BasicProductCategories.FirstOrDefaultAsync(c => c.Id == command.Id, ct);

        if (category is null)
        {
            return;
        }

        category.Deleted = DateTime.Now;

        context.BasicProductCategories.Update(category);

        await context.SaveChangesAsync(ct);
    }
}