using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Notification.Interfaces;

public interface INotificationRepository : IRepository<NotificationModel>
{
    Task<Pagination<List<NotificationModel>>> GetAllWithPaginationAsync(
        int page,
        int perPage,
        CancellationToken cancellationToken = default);
}