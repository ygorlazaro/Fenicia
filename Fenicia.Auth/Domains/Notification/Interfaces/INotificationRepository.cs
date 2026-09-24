using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.Notification.Interfaces;

/// <summary>
/// Interface for the notification repository, providing methods to manage notifications in the data store.
/// </summary>
public interface INotificationRepository : IRepository<NotificationModel>
{
    /// <summary>
    ///   Retrieves a paginated list of notifications for a specific company and user.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="page">The page number.</param>
    /// <param name="perPage">The number of items per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paginated list of notifications.</returns>
    Task<IEnumerable<NotificationModel>> GetAllAsync(Guid companyId, Guid userId, int page = 1, int perPage = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///  Retrieves a specific notification by its ID, company ID, and user ID.
    /// </summary>
    /// <param name="id">The ID of the notification.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The notification if found, otherwise null.</returns>
    Task<NotificationModel?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken);
}
