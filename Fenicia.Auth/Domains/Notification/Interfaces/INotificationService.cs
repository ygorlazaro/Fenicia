using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Notification;

namespace Fenicia.Auth.Domains.Notification.Interfaces;

public interface INotificationService
{
    Task<Pagination<List<NotificationResponse>>> GetAllAsync(
        Guid companyId,
        Guid userId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default);

    Task<NotificationResponse?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken = default);

    Task<NotificationResponse> AddAsync(
        NotificationRequest request,
        CancellationToken cancellationToken = default);

    Task<NotificationResponse?> UpdateAsync(
        NotificationRequest request,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
