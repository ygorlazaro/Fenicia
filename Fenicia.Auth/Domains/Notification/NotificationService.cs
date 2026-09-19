using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Auth.Domains.NotificationHistory.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Notification;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationService(
    NotificationMapper mapper,
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
        var result = notifications.Select(mapper.MapNotificationResponse).ToList();

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

        var result = mapper.MapNotificationResponse(notification);
        result.IsRead = await notificationHistoryService.IsReadAsync(id, cancellationToken);

        return result;
    }

    public async Task<NotificationResponse> AddAsync(
        NotificationRequest command,
        CancellationToken cancellationToken = default)
    {
        var notification = new NotificationModel
        {
            Title = command.Title,
            Description = command.Description,
            Date = DateTime.UtcNow,
            ImageUrl = command.ImageUrl
        };

        var created = await repository.InsertAsync(notification, cancellationToken);

        return mapper.MapNotificationResponse(created);
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

        return mapper.MapNotificationResponse(notification);
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
}