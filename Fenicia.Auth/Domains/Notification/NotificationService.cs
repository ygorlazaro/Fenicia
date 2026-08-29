using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationService(NotificationRepository repository)
{
    public async Task<Pagination<List<GetAllNotificationsResponse>>> GetAllAsync(int page, int perPage, CancellationToken ct)
    {
        var result = await repository.GetAllWithPaginationAsync(page, perPage, ct);

        return new Pagination<List<GetAllNotificationsResponse>>(result.Data.Select(n => n.MapToGetAllNotificationsResponse()).ToList(), result.Total, page, perPage);
    }

    public async Task<GetNotificationByIdResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(id, ct);

        return notification is null ? null : notification.MapToGetNotificationByIdResponse();
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

        var created = await repository.InsertAsync(notification, ct);

        return new AddNotificationResponse(created.Id);
    }

    public async Task<UpdateNotificationResponse?> UpdateAsync(UpdateNotificationCommand command, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(command.Id, ct);

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

        await repository.UpdateAsync(notification.Id, notification, ct);

        return new UpdateNotificationResponse(notification.Id);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(id, ct);

        if (notification is null)
        {
            return false;
        }

        notification.Deleted = DateTime.UtcNow;
        await repository.UpdateAsync(notification.Id, notification, ct);

        return true;
    }

    public async Task<bool> MarkAsReadAsync(Guid id, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(id, ct);

        if (notification is null)
        {
            return false;
        }

        notification.Read = true;
        await repository.UpdateAsync(notification.Id, notification, ct);

        return true;
    }
}
