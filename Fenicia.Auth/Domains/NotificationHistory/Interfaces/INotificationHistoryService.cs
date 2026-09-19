namespace Fenicia.Auth.Domains.NotificationHistory.Interfaces;

public interface INotificationHistoryService
{
    Task<bool> MaskReadStatusAsync(Guid notificationId, Guid userId, Guid companyId, bool readStatus, CancellationToken
        cancellationToken);

    Task<bool> IsReadAsync(Guid notificationId, CancellationToken cancellationToken);
}