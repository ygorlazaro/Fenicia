using Fenicia.Auth.Domains.Notification.DTOs;
using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification;

public class NotificationService(INotificationRepository repository) : INotificationService
{
    public async Task<Pagination<List<GetAllNotificationsResponse>>> GetAllAsync(GetAllNotificationsQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query().OrderByDescending(n => n.Date);
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);

        var totalTask = filteredQuery.CountAsync(cancellationToken);
        var itemsTask = filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);

        await Task.WhenAll(totalTask, itemsTask);

        return new Pagination<List<GetAllNotificationsResponse>>([.. itemsTask.Result.Select(n => n.MapToGetAllNotificationsResponse())], totalTask.Result, query.Page, query.PerPage);
    }

    public async Task<GetNotificationByIdResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);

        return notification?.MapToGetNotificationByIdResponse();
    }

    public async Task<AddNotificationResponse> AddAsync(AddNotificationCommand command, Guid companyId, CancellationToken cancellationToken = default)
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

        var created = await repository.InsertAsync(notification, cancellationToken);

        return new AddNotificationResponse(created.Id);
    }

    public async Task<UpdateNotificationResponse?> UpdateAsync(UpdateNotificationCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(command.Id, cancellationToken);

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

        await repository.UpdateAsync(notification.Id, notification, cancellationToken);

        return new UpdateNotificationResponse(notification.Id);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default)
    {
        var notification = await repository.GetByIdAsync(id, cancellationToken);

        if (notification is null)
        {
            return false;
        }

        notification.Deleted = DateTime.UtcNow;
        notification.CompanyId = companyId;
        await repository.UpdateAsync(notification.Id, notification, cancellationToken);

        return true;
    }
}
