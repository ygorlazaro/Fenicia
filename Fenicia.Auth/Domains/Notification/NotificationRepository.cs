using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.Notification;

/// <summary>
/// Implementation of the notification repository, providing methods to manage notifications in the data store.
/// </summary>
/// <param name="context"></param>
public class NotificationRepository(DefaultContext context)
    : Repository<NotificationModel>(context), INotificationRepository
{
    /// <summary>
    ///  Retrieves a paginated list of notifications for a specific company and user.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paginated list of notifications.</returns>
    public async Task<IEnumerable<NotificationModel>> GetAllAsync(Guid companyId, Guid userId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        var query = from n in DbSet
                    join nh in context.AuthNotificationHistory on n.Id equals nh.NotificationId
                    where nh.CompanyId == companyId || nh.UserId == userId
                    orderby n.Date descending
                    select n;

        return await query
            .Skip((perPage - 1) * page)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves a specific notification by its ID, company ID, and user ID.
    /// </summary>
    /// <param name="id">The ID of the notification.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The notification if found, otherwise null.</returns>
    public Task<NotificationModel?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken)
    {
        var query = from n in DbSet
                    join nh in context.AuthNotificationHistory on n.Id equals nh.NotificationId
                    where (nh.CompanyId == companyId || nh.UserId == userId)
                    && n.Id == id
                    orderby n.Date descending
                    select n;

        return query.FirstOrDefaultAsync(cancellationToken);
    }
}
