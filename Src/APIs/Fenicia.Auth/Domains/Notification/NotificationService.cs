using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationService(NotificationRepository repository)
{
    public async Task<Pagination<List<GetAllNotificationsResponse>>> GetAllAsync(int page, int perPage, CancellationToken ct)
    {
        var result = await repository.GetAllWithPaginationAsync(page, perPage, ct);

        return new Pagination<List<GetAllNotificationsResponse>>(result.Data.Select(n => n.MapToGetAllNotificationsResponse()).ToList(), result.Total, page, perPage);
    }

    public async Task<GetNotificationByIdResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(id, ct);

        return notification is null ? null : notification.MapToGetNotificationByIdResponse();
    }

    public async Task<AddNotificationResponse> AddAsync(AddNotificationCommand command, Guid companyId, CancellationToken ct)
    {
        var notification = new NotificationModel
        {
            Title = command.Title,
            Description = command.Description,
            Date = command.Date ?? DateTime.UtcNow,
            ImageUrl = command.ImageUrl,
            Read = false,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(notification, ct);

        return new AddNotificationResponse(created.Id);
    }

    public async Task<UpdateNotificationResponse?> UpdateAsync(UpdateNotificationCommand command, Guid companyId, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(command.Id, ct);

        if (notification is null)
        {
            return null;
        }

        notification.Title = command.Title;
        notification.Description = command.Description;
        notification.Date = command.Date ?? notification.Date;
        notification.ImageUrl = command.ImageUrl;
        notification.CompanyId = companyId;

        if (command.IsRead.HasValue)
        {
            notification.Read = command.IsRead.Value;
        }

        await repository.UpdateAsync(notification.Id, notification, ct);

        return new UpdateNotificationResponse(notification.Id);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid companyId, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(id, ct);

        if (notification is null)
        {
            return false;
        }

        notification.Deleted = DateTime.UtcNow;
        notification.CompanyId = companyId;
        await repository.UpdateAsync(notification.Id, notification, ct);

        return true;
    }
}
