using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Update;

public class UpdatePositionHandler(BasicContext context)
{
    public async Task<UpdatePositionResponse?> Handle(UpdatePositionCommand command, CancellationToken ct)
    {
        var position = await context.Positions.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;

        context.Positions.Update(position);

        await context.SaveChangesAsync(ct);

        return new UpdatePositionResponse(position.Id, position.Name);
    }
}