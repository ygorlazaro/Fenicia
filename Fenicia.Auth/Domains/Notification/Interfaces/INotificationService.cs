using Fenicia.Common;
using Fenicia.Common.DTOs.Auth.Notification;

namespace Fenicia.Auth.Domains.Notification.Interfaces;

/// <summary>
/// Interface for the notification service, providing methods to manage notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Retrieves a paginated list of all notifications for a specific user.
    /// </summary>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="query">The pagination query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The paginated list of notifications.</returns>
    Task<Pagination<IEnumerable<NotificationResponse>>> GetAllAsync(Guid companyId,
        Guid userId,
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a specific notification by its ID, company ID, and user ID.
    /// </summary>
    /// <param name="id">The ID of the notification.</param>
    /// <param name="companyId">The ID of the company.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The notification if found, otherwise null.</returns>
    Task<NotificationResponse?> GetByIdAsync(Guid id, Guid companyId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new notification to the system.
    /// </summary>
    /// <param name="request">The request containing the notification details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created notification.</returns>
    Task<NotificationResponse> AddAsync(
        NotificationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing notification in the system.
    /// </summary>
    /// <param name="request">The request containing the updated notification details.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated notification.</returns>
    Task<NotificationResponse?> UpdateAsync(
        NotificationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a notification from the system.
    /// </summary>
    /// <param name="id">The ID of the notification to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean indicating whether the deletion was successful.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
