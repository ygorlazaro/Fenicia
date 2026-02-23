using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Delete;

public class DeleteProductHandler(BasicContext context)
{
    public async Task Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await context.Products.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product is null) return;

        product.Deleted = DateTime.Now;

        context.Products.Update(product);

        await context.SaveChangesAsync(ct);
    }
}
