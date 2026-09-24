using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Auth.Domains.NotificationHistory.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Notification;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationService(
    INotificationRepository repository,
    INotificationHistoryService notificationHistoryService) : INotificationService
{
    public async Task<Pagination<List<NotificationResponse>>> GetAllAsync(
        Guid companyId,
        Guid userId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        var notifications = (List<NotificationModel>)[.. await repository.GetAllAsync(companyId, userId, page, perPage, cancellationToken)];
        var total = await repository.CountAsync(cancellationToken);
        var result = notifications.Select(MapNotificationResponse).ToList();

        foreach (var notification in result)
        {
            notification.IsRead = await notificationHistoryService.IsReadAsync(notification.Id, cancellationToken);
        }

        return new Pagination<List<NotificationResponse>>(
            [.. result],
            total,
            page,
            perPage);
    }

    public async Task<NotificationResponse?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, companyId, userId, cancellationToken);

        if (notification is null)
        {
            return null;
        }

        var result = MapNotificationResponse(notification);
        result.IsRead = await notificationHistoryService.IsReadAsync(id, cancellationToken);

        return result;
    }

    public async Task<NotificationResponse> AddAsync(
        NotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var notification = new NotificationModel
        {
            Title = request.Title,
            Description = request.Description,
            Date = DateTime.UtcNow,
            ImageUrl = request.ImageUrl
        };

        var created = await repository.InsertAsync(notification, cancellationToken);

        return MapNotificationResponse(created);
    }

    public async Task<NotificationResponse?> UpdateAsync(
        NotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(request.Id, cancellationToken);

        if (notification is null)
        {
            return null;
        }

        notification.Title = request.Title;
        notification.Description = request.Description;
        notification.ImageUrl = request.ImageUrl;

        await repository.UpdateAsync(notification.Id, notification, cancellationToken);

        return MapNotificationResponse(notification);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);

        if (notification is null)
        {
            return false;
        }

        notification.Deleted = DateTime.UtcNow;
        await repository.UpdateAsync(notification.Id, notification, cancellationToken);

        return true;
    }

    private static NotificationResponse MapNotificationResponse(NotificationModel notification)
    {
        return new NotificationResponse(
            notification.Id,
            notification.Title,
            notification.Description,
            notification.Date,
            notification.ImageUrl);
    }
}
