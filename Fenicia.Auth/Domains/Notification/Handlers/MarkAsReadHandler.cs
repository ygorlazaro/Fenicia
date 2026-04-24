using Fenicia.Auth.Domains.Notification.Commands;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification.Handlers;

public class MarkAsReadHandler(DefaultContext db)
{
    public async Task<bool> Handle(MarkAsReadCommand command, CancellationToken ct)
    {
        var notification = await db.AuthNotifications
            .FirstOrDefaultAsync(n => n.Id == command.Id, ct);

        if (notification is null)
        {
            return false;
        }

        notification.Read = true;
        db.AuthNotifications.Update(notification);
        await db.SaveChangesAsync(ct);

        return true;
    }
}
