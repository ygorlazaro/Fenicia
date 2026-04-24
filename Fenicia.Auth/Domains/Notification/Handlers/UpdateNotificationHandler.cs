using Fenicia.Auth.Domains.Notification.Commands;
using Fenicia.Auth.Domains.Notification.Responses;
using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification.Handlers;

public class UpdateNotificationHandler(DefaultContext db)
{
    public async Task<UpdateNotificationResponse?> Handle(UpdateNotificationCommand command, CancellationToken ct)
    {
        var notification = await db.AuthNotifications
            .FirstOrDefaultAsync(n => n.Id == command.Id, ct);

        if (notification is null)
        {
            return null;
        }

        notification.Title = command.Title;
        notification.Description = command.Description;
        notification.Date = command.Date ?? notification.Date;
        notification.ImageUrl = command.ImageUrl;

        if (command.Read.HasValue)
        {
            notification.Read = command.Read.Value;
        }

        db.AuthNotifications.Update(notification);
        await db.SaveChangesAsync(ct);

        return new UpdateNotificationResponse(notification.Id);
    }
}
