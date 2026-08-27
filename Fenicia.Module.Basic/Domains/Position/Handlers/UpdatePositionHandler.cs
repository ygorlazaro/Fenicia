using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Position.DTOs.Responses;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

public class UpdatePositionHandler(DefaultContext db) : IRequestHandler<UpdatePositionCommand, UpdatePositionResponse?>
{

    public async Task<UpdatePositionResponse?> Handle(UpdatePositionCommand command, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;

        db.BasicPositions.Update(position);

        await db.SaveChangesAsync(ct);

        return new UpdatePositionResponse(position.Id, position.Name);
    }
}
