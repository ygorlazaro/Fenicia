using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Product.Delete;

public class DeleteProductHandler(DefaultContext context)
{
    public async Task Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await context.BasicProducts.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (product is null)
        {
            return;
        }

        product.Deleted = DateTime.Now;

        context.BasicProducts.Update(product);

        await context.SaveChangesAsync(ct);
    }
}
