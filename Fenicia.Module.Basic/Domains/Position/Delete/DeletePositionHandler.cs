using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Delete;

public class DeletePositionHandler(BasicContext context)
{
    public async Task Handle(DeletePositionCommand command, CancellationToken ct)
    {
        var position = await context.Positions.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (position is null)
        {
            return;
        }

        position.Deleted = DateTime.Now;

        context.Positions.Update(position);

        await context.SaveChangesAsync(ct);
    }
}
