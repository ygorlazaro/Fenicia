using Fenicia.Auth.Domains.NotificationHistory.Interfaces;

namespace Fenicia.Auth.Domains.NotificationHistory;

/// <summary>
/// Implementation of the notification history service, providing methods to manage notification history.
/// </summary>
/// <param name="repository"></param>
public class NotificationHistoryService(INotificationHistoryRepository repository) : INotificationHistoryService
{
    /// <summary>
    /// Marks the read status of a notification for a specific user and company.
    /// </summary>
    /// <param name="notificationId">The ID of the notification.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="readStatus">The read status to be set (true for read, false for unread).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean indicating whether the operation was successful.</returns>
    public Task<bool> MaskReadStatusAsync(Guid notificationId, Guid userId, Guid companyId, bool readStatus,
        CancellationToken cancellationToken)
    {
        return repository.MaskReadStatusAsync(notificationId, userId, companyId, readStatus, cancellationToken);
    }

    /// <summary>
    /// Checks if a notification has been read by a specific user.
    /// </summary>
    /// <param name="notificationId">The ID of the notification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean indicating whether the notification has been read.</returns>
    public Task<bool> IsReadAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        return repository.IsReadAsync(notificationId, cancellationToken);
    }
}
