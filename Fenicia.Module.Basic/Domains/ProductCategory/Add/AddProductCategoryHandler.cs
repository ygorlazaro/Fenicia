using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Add;

public class AddProductCategoryHandler(DefaultContext context)
{
    public async Task<AddProductCategoryResponse> Handle(AddProductCategoryCommand command, CancellationToken ct)
    {
        var category = new BasicProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name
        };

        context.BasicProductCategories.Add(category);

        await context.SaveChangesAsync(ct);

        return new AddProductCategoryResponse(category.Id, category.Name);
    }
}
