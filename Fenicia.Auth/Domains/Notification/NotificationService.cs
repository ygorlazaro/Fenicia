using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationService(DefaultContext db)
{
    public async Task<Pagination<List<GetAllNotificationsResponse>>> GetAllAsync(int page, int perPage, CancellationToken ct)
    {
        var total = await db.AuthNotifications.CountAsync(ct);

        var notifications = await db.AuthNotifications
            .OrderByDescending(n => n.Date)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);

        var response = notifications.Select(n => new GetAllNotificationsResponse(
            n.Id,
            n.Title,
            n.Description,
            n.Date,
            n.ImageUrl,
            n.Read)).ToList();

        return new Pagination<List<GetAllNotificationsResponse>>(response, total, page, perPage);
    }

    public async Task<GetNotificationByIdResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var notification = await db.AuthNotifications
                .FirstOrDefaultAsync(n => n.Id == id, ct);

        if (notification is null)
        {
            return null;
        }

        return new GetNotificationByIdResponse(
            notification.Id,
            notification.Title,
            notification.Description,
            notification.Date,
            notification.ImageUrl,
            notification.Read);
    }

    public async Task<AddNotificationResponse> AddAsync(AddNotificationCommand command, CancellationToken ct)
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

    public async Task<UpdateNotificationResponse?> UpdateAsync(UpdateNotificationCommand command, CancellationToken ct)
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

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var notification = await db.AuthNotifications
                .FirstOrDefaultAsync(n => n.Id == id, ct);

        if (notification is null)
        {
            return false;
        }

        notification.Deleted = DateTime.UtcNow;
        db.AuthNotifications.Update(notification);
        await db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> MarkAsReadAsync(Guid id, CancellationToken ct)
    {
        var notification = await db.AuthNotifications
                .FirstOrDefaultAsync(n => n.Id == id, ct);

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
