using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.GetById;

public class GetProductByIdHandler(DefaultContext context)
{
    public async Task<GetProductByIdResponse?> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var product = await context.BasicProducts
            .Select(p => new GetProductByIdResponse(p.Id, p.Name, p.CostPrice, p.SalesPrice, p.Quantity, p.CategoryId, p.CategoryModel.Name))
            .FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        return product;
    }
}
