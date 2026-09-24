using Fenicia.Auth.Domains.NotificationHistory.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.NotificationHistory;

/// <summary>
/// Implementation of the notification history repository, providing methods to manage notification history.
/// </summary>
/// <param name="context"></param>/
public class NotificationHistoryRepository(DbContext context) : Repository<NotificationHistoryModel>(context), INotificationHistoryRepository
{
    /// <summary>
    /// Marks the read status of a notification for a specific user and company.
    /// </summary>
    /// <param name="notificationId">The ID of the notification.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="readStatus">The read status to be set (true for read, false for unread).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean indicating whether the operation was successful.</returns>
    public async Task<bool> MaskReadStatusAsync(Guid notificationId, Guid userId, Guid companyId, bool readStatus,
        CancellationToken cancellationToken)
    {
        var query = from nh in DbSet
                    where nh.NotificationId == notificationId
                        && nh.UserId == userId
                        && nh.CompanyId == companyId
                    select nh;

        var result = await query.FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return false;
        }

        result.IsRead = readStatus;
        Context.Entry(result).State = EntityState.Modified;

        await SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Checks if a notification has been read by a specific user.
    /// </summary>
    /// <param name="notificationId">The ID of the notification.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean indicating whether the notification has been read.</returns>
    public Task<bool> IsReadAsync(Guid notificationId, CancellationToken cancellationToken)
    {
        return DbSet.AnyAsync(nh => nh.Id == notificationId && nh.IsRead, cancellationToken);
    }
}
