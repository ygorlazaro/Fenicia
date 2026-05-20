using Fenicia.Auth.Domains.Notification.Commands;
using Fenicia.Common.Data.Contexts;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification.Handlers;

public class DeleteNotificationHandler(DefaultContext db) : IRequestHandler<DeleteNotificationCommand>
{
    public async Task Handle(DeleteNotificationCommand command, CancellationToken ct)
    {
        var notification = await db.AuthNotifications
            .FirstOrDefaultAsync(n => n.Id == command.Id, ct);

        if (notification is null)
        {
            return;
        }

        notification.Deleted = DateTime.UtcNow;
        db.AuthNotifications.Update(notification);
        await db.SaveChangesAsync(ct);
    }
}
