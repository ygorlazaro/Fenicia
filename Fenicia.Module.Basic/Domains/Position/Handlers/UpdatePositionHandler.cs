using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Commands;
using Fenicia.Module.Basic.Domains.Position.Responses;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

/// <summary>
/// Handler for updating an existing position.
/// </summary>
/// <remarks>
/// This handler:
/// 1. Queries the database for a position with the specified ID
/// 2. Returns null if position is not found
/// 3. Updates the position Name with the new value
/// 4. Marks the entity as modified and saves changes
/// 5. Returns the updated position data
/// </remarks>
public class UpdatePositionHandler(DefaultContext db)
{
    /// <summary>
    /// Handles updating an existing position.
    /// </summary>
    /// <param name="command">The command containing updated position data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated position response if found, null otherwise.</returns>
    public async Task<UpdatePositionResponse?> Handle(UpdatePositionCommand command, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == command.Id,
            ct);

        if (position is null)
        {
            return null;
        }

        position.Name = command.Name;

        db.BasicPositions.Update(position);

        await db.SaveChangesAsync(ct);

        return new UpdatePositionResponse(position.Id,
            position.Name);
    }
}
