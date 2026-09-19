using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.NotificationHistory.Interfaces;

public interface INotificationHistoryRepository : IRepository<NotificationHistoryModel>
{
    Task<bool> MaskReadStatusAsync(Guid notificationId, Guid userId, Guid companyId, bool readStatus, CancellationToken
        cancellationToken);

    Task<bool> IsReadAsync(Guid notificationId, CancellationToken cancellationToken);
}