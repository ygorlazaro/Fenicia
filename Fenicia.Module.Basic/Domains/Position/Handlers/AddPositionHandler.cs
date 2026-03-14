using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Responses;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

/// <summary>
///     Handler for creating new positions.
/// </summary>
/// <remarks>
///     This handler:
///     1. Creates a new PositionModel instance
///     2. Sets the position Id and Name from the command
///     3. Adds the position to the database
///     4. Saves changes to persist the new position
///     5. Returns the created position data
/// </remarks>
public class AddPositionHandler(DefaultContext db)
{
    /// <summary>
    ///     Handles the creation of a new position.
    /// </summary>
    /// <param name="command">The command containing position data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created position response.</returns>
    public async Task<AddPositionResponse> Handle(AddPositionCommand command, CancellationToken ct)
    {
        var position = new PositionModel { Id = command.Id, Name = command.Name };

        db.BasicPositions.Add(position);

        await db.SaveChangesAsync(ct);

        return new AddPositionResponse(position.Id, position.Name);
    }
}