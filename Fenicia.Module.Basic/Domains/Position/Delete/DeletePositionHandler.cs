using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Delete;

public class DeletePositionHandler(DefaultContext context)
{
    public async Task Handle(DeletePositionCommand command, CancellationToken ct)
    {
        var position = await context.BasicPositions.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (position is null)
        {
            return;
        }

        position.Deleted = DateTime.Now;

        context.BasicPositions.Update(position);

        await context.SaveChangesAsync(ct);
    }
}
