using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common;

namespace Fenicia.Auth.Domains.Notification.Interfaces;

public interface INotificationService
{
    Task<Pagination<List<GetAllNotificationsResponse>>> GetAllAsync(GetAllNotificationsQuery query, CancellationToken cancellationToken = default);

    Task<GetNotificationByIdResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AddNotificationResponse> AddAsync(AddNotificationCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdateNotificationResponse?> UpdateAsync(UpdateNotificationCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);
}
