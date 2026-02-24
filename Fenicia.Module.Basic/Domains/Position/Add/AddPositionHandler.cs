using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;

namespace Fenicia.Module.Basic.Domains.Position.Add;

public class AddPositionHandler(BasicContext context)
{
    public async Task<AddPositionResponse> Handle(AddPositionCommand command, CancellationToken ct)
    {
        var position = new PositionModel
        {
            Id = command.Id,
            Name = command.Name
        };

        context.Positions.Add(position);

        await context.SaveChangesAsync(ct);

        return new AddPositionResponse(position.Id, position.Name);
    }
}