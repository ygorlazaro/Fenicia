using Fenicia.Auth.Domains.Notification.Commands;
using Fenicia.Auth.Domains.Notification.Responses;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Notification.Handlers;

public class AddNotificationHandler(DefaultContext db)
{
    public async Task<AddNotificationResponse> Handle(AddNotificationCommand command, CancellationToken ct)
    {
        var notification = new NotificationModel
        {
            Title = command.Title,
            Description = command.Description,
            Date = command.Date ?? DateTime.UtcNow,
            ImageUrl = command.ImageUrl,
            Read = false
        };

        db.AuthNotifications.Add(notification);
        await db.SaveChangesAsync(ct);

        return new AddNotificationResponse(notification.Id);
    }
}
