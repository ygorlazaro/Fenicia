using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Position.Commands;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.Position.Handlers;

/// <summary>
/// Handler for soft-deleting a position.
/// </summary>
/// <remarks>
/// This handler:
/// 1. Queries the database for a position with the specified ID
/// 2. Returns silently if position is not found
/// 3. Sets the Deleted timestamp to current time (soft delete)
/// 4. Marks the entity as modified and saves changes
/// </remarks>
public class DeletePositionHandler(DefaultContext db)
{
    /// <summary>
    /// Handles soft-deleting a position by setting its Deleted timestamp.
    /// </summary>
    /// <param name="command">The command containing the position ID to delete.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task Handle(DeletePositionCommand command, CancellationToken ct)
    {
        var position = await db.BasicPositions.FirstOrDefaultAsync(p => p.Id == command.Id,
            ct);

        if (position is null)
        {
            return;
        }

        position.Deleted = DateTime.Now;

        db.BasicPositions.Update(position);

        await db.SaveChangesAsync(ct);
    }
}
