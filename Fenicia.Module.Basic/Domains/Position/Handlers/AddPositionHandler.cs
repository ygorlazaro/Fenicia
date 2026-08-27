using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.DTOs.Commands;
using Fenicia.Module.Basic.Domains.Position.DTOs.Responses;

using MediatR;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

public class AddPositionHandler(DefaultContext db) : IRequestHandler<AddPositionCommand, AddPositionResponse>
{

    public async Task<AddPositionResponse> Handle(AddPositionCommand command, CancellationToken ct)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name
        };

        db.BasicPositions.Add(position);

        await db.SaveChangesAsync(ct);

        return new AddPositionResponse(position.Id, position.Name);
    }
}
