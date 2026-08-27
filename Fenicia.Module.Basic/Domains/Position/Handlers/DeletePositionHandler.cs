using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.DTOs.Commands;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

public class DeletePositionHandler(DefaultContext db) : IRequestHandler<DeletePositionCommand>
{

    public async Task Handle(DeletePositionCommand command, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (position is null)
        {
            return;
        }

        position.Deleted = DateTime.Now;

        db.BasicPositions.Update(position);

        await db.SaveChangesAsync(ct);
    }
}
