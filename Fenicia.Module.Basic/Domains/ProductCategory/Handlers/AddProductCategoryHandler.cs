using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.ProductCategory.Commands;
using Fenicia.Module.Basic.Domains.ProductCategory.Responses;
using MediatR;

namespace Fenicia.Module.Basic.Domains.ProductCategory.Handlers;

public class AddProductCategoryHandler(DefaultContext db) : IRequestHandler<AddProductCategoryCommand, AddProductCategoryResponse>
{

    public async Task<AddProductCategoryResponse> Handle(AddProductCategoryCommand command, CancellationToken ct)
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
}