using System.Net.Mime;
using Fenicia.Auth.Domains.Notification.Interfaces;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Notification;

/// <summary>
/// Represents a controller for managing notifications.
/// </summary>
/// <param name="notificationService"></param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    /// <summary>
    /// Gets all notifications for the authenticated user.
    /// </summary>
    /// <param name="query">Pagination parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of notifications</returns>
    /// <response code="200">Notifications returned successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<IEnumerable<NotificationResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<IEnumerable<NotificationResponse>>>> GetAsync(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var companyId = ClaimReader.CompanyId(User);
            var userId = ClaimReader.UserId(User);
            var notifications = await notificationService.GetAllAsync(companyId, userId, query, cancellationToken);
            return Ok(notifications);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets a specific notification by ID.
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Notification data</returns>
    /// <response code="200">Notification found</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Notification not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotificationResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NotificationResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var companyId = ClaimReader.CompanyId(User);
            var userId = ClaimReader.UserId(User);
            var notification = await notificationService.GetByIdAsync(id, companyId, userId, cancellationToken);
            return notification is null ? NotFound() : Ok(notification);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new notification.
    /// </summary>
    /// <param name="request">Notification data (title, description, date, image)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created notification data</returns>
    /// <response code="201">Notification created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NotificationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<NotificationResponse>> PostAsync(
        [FromBody] NotificationRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var notification = await notificationService.AddAsync(request, cancellationToken);
            return new CreatedResult(string.Empty, notification);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Updates an existing notification.
    /// </summary>
    /// <param name="request">Updated notification data (title, description, date, image, read)</param>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated notification data</returns>
    /// <response code="200">Notification updated successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Notification not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NotificationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<NotificationResponse>> PatchAsync(
        [FromBody] NotificationRequest request,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            request.Id = id;
            var notification = await notificationService.UpdateAsync(
                request,
                cancellationToken);
            return notification switch
            {
                null => NotFound(),
                _ => Ok(notification)
            };
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Removes a notification (soft delete).
    /// </summary>
    /// <param name="id">Notification ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content (204) if removed successfully</returns>
    /// <response code="204">Notification removed successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await notificationService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
