using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Position.Add;

public class AddPositionHandler(DefaultContext context)
{
    public async Task<AddPositionResponse> Handle(AddPositionCommand command, CancellationToken ct)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name
        };

        context.BasicPositions.Add(position);

        await context.SaveChangesAsync(ct);

        return new AddPositionResponse(position.Id, position.Name);
    }
}
