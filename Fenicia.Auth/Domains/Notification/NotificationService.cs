using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Auth.Domains.NotificationHistory.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.DTOs.Auth.Notification;

namespace Fenicia.Auth.Domains.Notification;

/// <summary>
/// Initializes a new instance of the <see cref="NotificationService"/> class.
/// </summary>
/// <param name="repository">The notification repository.</param>
/// <param name="notificationHistoryService">The notification history service.</param>
public class NotificationService(
    INotificationRepository repository,
    INotificationHistoryService notificationHistoryService) : INotificationService
{
    /// <summary>
    /// Retrieves a paginated list of all notifications for a specific user.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="query">The pagination query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paginated list of notifications.</returns>
    public async Task<Pagination<IEnumerable<NotificationResponse>>> GetAllAsync(Guid companyId,
        Guid userId,
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        var notifications = await repository.GetAllAsync(companyId, userId, query.Page, query.PerPage, cancellationToken);
        var total = await repository.CountAsync(cancellationToken);
        var result = notifications.Select(NotificationMapper.MapNotificationResponse).ToList();

        foreach (var notification in result)
        {
            notification.IsRead = await notificationHistoryService.IsReadAsync(notification.Id, cancellationToken);
        }

        return new Pagination<IEnumerable<NotificationResponse>>(
            [.. result],
            total,
            query.Page,
            query.PerPage);
    }

    /// <summary>
    /// Retrieves a specific notification by its ID for a specific user.
    /// </summary>
    /// <param name="id">The ID of the notification.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The notification if found, otherwise null.</returns>
    public async Task<NotificationResponse?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, companyId, userId, cancellationToken);

        if (notification is null)
        {
            return null;
        }

        var result = NotificationMapper.MapNotificationResponse(notification);
        result.IsRead = await notificationHistoryService.IsReadAsync(id, cancellationToken);

        return result;
    }

    /// <summary>
    /// Adds a new notification.
    /// </summary>
    /// <param name="request">The notification request containing the details of the notification to be added.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added notification response.</returns>
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

        return NotificationMapper.MapNotificationResponse(created);
    }

    /// <summary>
    /// Updates an existing notification.
    /// </summary>
    /// <param name="request">The notification request containing the details of the notification to be updated.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated notification response if successful, otherwise null.</returns>
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

        return NotificationMapper.MapNotificationResponse(notification);
    }

    /// <summary>
    /// Deletes a notification.
    /// </summary>
    /// <param name="id">The ID of the notification to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean indicating whether the notification was deleted successfully.</returns>
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
