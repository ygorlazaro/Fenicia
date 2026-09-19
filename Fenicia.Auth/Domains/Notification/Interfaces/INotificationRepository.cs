using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Notification.Interfaces;

public interface INotificationRepository : IRepository<NotificationModel>
{
    Task<IEnumerable<NotificationModel>> GetAllAsync(Guid companyId, Guid userId, int page = 1, int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<NotificationModel?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken);
}