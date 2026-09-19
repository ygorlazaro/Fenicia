using Fenicia.Auth.Domains.NotificationHistory.Interfaces;

namespace Fenicia.Auth.Domains.NotificationHistory;

public class NotificationHistoryService(INotificationHistoryRepository repository) : INotificationHistoryService
{
    public Task<bool> MaskReadStatusAsync(Guid notificationId, Guid userId, Guid companyId, bool readStatus,
        CancellationToken cancellationToken)
    {
        return repository.MaskReadStatusAsync(notificationId, userId, companyId, readStatus, cancellationToken);
    }

    public Task<bool> IsReadAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        return repository.IsReadAsync(notificationId, cancellationToken);
    }
}