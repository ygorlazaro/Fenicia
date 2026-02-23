using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Add;

public class AddProductCategoryHandler(BasicContext context)
{
    public async Task<ProductCategoryResponse> Handle(AddProductCategoryCommand command, CancellationToken ct)
    {
        var category = new ProductCategoryModel
        {
            Id = command.Id,
            Name = command.Name
        };

        context.ProductCategories.Add(category);

        await context.SaveChangesAsync(ct);

        return new ProductCategoryResponse(category.Id, category.Name);
    }
}
